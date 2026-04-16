<template>
	<div class="flex-column">
		<div class="filter-container">
			<el-card shadow="never" class="boby-small" style="height: 100%">
				<div slot="header" class="clearfix">
					<span>模块表</span>
				</div>
				<div>
					<el-row :gutter="2">
						<el-col :span="21">
						<el-button type="primary"  size="small" icon="el-icon-plus" @click="handleCreate">添加</el-button>
							<el-button type="primary" size="small"  icon="el-icon-edit" @click="handleUpdate">编辑</el-button>
							<el-button type="danger" size="small" icon="el-icon-delete" @click="handleDelete">删除</el-button>
						
						</el-col>
						<el-col :span="3">
						</el-col>
					</el-row>
				</div>
			</el-card>
		</div>
		
		
		
		<div class="app-container fh">
			
				<el-table ref="mainTable" :key="tableKey" :data="list" v-loading="listLoading" row-key="id" border fit
					stripe highlight-current-row style="width: 100%;" height="calc(100% - 1px)" @row-click="rowClick"
					@selection-change="handleSelectionChange"
					align="left">
					<el-table-column type="selection" align="center" width="55"></el-table-column>

					<el-table-column :show-overflow-tooltip="true" min-width="140px" :label="'模块名称'" header-align="left"
						align="left" style="text-align:left" prop="name" sortable>
						<template slot-scope="scope">
							<span>{{scope.row.name}}</span>
						</template>
					</el-table-column>
					<el-table-column :show-overflow-tooltip="true" min-width="230px" :label="'模块标识'" prop="code"
						sortable>
						<template slot-scope="scope">
							<span>{{scope.row.code}}</span>
						</template>
					</el-table-column>
					<el-table-column :show-overflow-tooltip="true" min-width="140px" :label="'URL'" prop="url" sortable>
						<template slot-scope="scope">
							<span>{{scope.row.url}}</span>
						</template>
					</el-table-column>


				</el-table>
			
				<!-- <el-card shadow="never" class="card-body-none fh" style="overflow-y: auto">

				<tree-table highlight-current-row :data="modulesTree" :columns="columns" border></tree-table>
			</el-card> -->


			<!--模块编辑对话框-->
			<el-dialog v-el-drag-dialog class="dialog-mini" width="500px" :title="textMap[dialogStatus]"
				:visible.sync="dialogFormVisible">
				<el-form :rules="rules" ref="dataForm" :model="temp" label-position="right" label-width="100px">
					<el-form-item size="small" :label="'Id'" prop="id" v-show="dialogStatus=='update'">
						<span>{{temp.id}}</span>
					</el-form-item>
					<el-form-item size="small" :label="'层级ID'" v-show="dialogStatus=='update'">
						<span>{{temp.cascadeId}}</span>
					</el-form-item>
					<el-form-item size="small" :label="'名称'" prop="name">
						<el-input v-model="temp.name"></el-input>
					</el-form-item>
					<el-form-item size="small" :label="'排序'">
						<el-input-number v-model="temp.sortNo" :min="1" :max="20"></el-input-number>
					</el-form-item>
					<el-form-item size="small" :label="'是否系统'" prop="isSys">
						<el-switch v-model="temp.isSys" active-color="#13ce66" inactive-color="#ff4949">
						</el-switch>
					</el-form-item>
					<el-form-item size="small" :label="'模块标识'">
						<el-input v-model="temp.code"></el-input>
					</el-form-item>
					<el-form-item size="small" :label="'图标'">
						<el-popover placement="top-start" width="400" trigger="click"
							>
							<el-input slot="reference"
								:class="temp.iconName ? `iconfont icon-${temp.iconName} custom-icon-input` : ''"
								v-model="temp.iconName"></el-input>
							<el-row class="selectIcon-box">
								<el-col :class="{'active': temp.iconName === item.font_class}" :span="3"
									v-for="(item,index) in iconData.glyphs" :key="index">
									<i :class="`${iconData.font_family} ${iconData.css_prefix_text}${item.font_class}`"
										@click="handleChangeTempIcon(item)"></i>
								</el-col>
							</el-row>
						</el-popover>
					</el-form-item>
					<el-form-item size="small" :label="'url'">
						<el-input v-model="temp.url"></el-input>
					</el-form-item>

					<el-form-item size="mini" :label="'上级机构'">

						<!-- <treeselect ref="modulesTree" :normalizer="normalizer" :disabled="treeDisabled"
							:options="modulesTreeRoot" :default-expand-level="3" :multiple="false" :open-on-click="true"
							:open-on-focus="true" :clear-on-select="true" v-model="dpSelectModule"></treeselect> -->
							
							<el-select class="filter-item" filterable v-model="temp.parentId"
								placeholder="Please select" style="width:200px">
								<el-option value label>请选择</el-option>
								<el-option v-for="item in  moduleOptions" :key="item.key"
									:label="item.display_name" :value="item.key"></el-option>
							</el-select>
					</el-form-item>
				</el-form>
				<div slot="footer">
					<el-button size="mini" @click="dialogFormVisible = false">取消</el-button>
					<el-button size="mini" v-if="dialogStatus=='create'" type="primary" @click="createData">确认</el-button>
					<el-button size="mini" v-else type="primary" @click="updateData">确认</el-button>
				</div>
			</el-dialog>

		</div>
	</div>

</template>

<script>
	import treeTable from '@/components/TreeTable'
	import Pagination from '@/components/Pagination'
	import {
		listToTreeSelect
	} from '@/utils'
	import extend from "@/extensions/delRows.js"
	import * as modules from '@/api/modules'
	import * as login from '@/api/login'
	import Treeselect from '@riophae/vue-treeselect'
	import '@riophae/vue-treeselect/dist/vue-treeselect.css'
	import waves from '@/directive/waves' // 水波纹指令
	import Sticky from '@/components/Sticky'
	import permissionBtn from '@/components/PermissionBtn'
	import elDragDialog from '@/directive/el-dragDialog'
	import iconData from '@/assets/public/css/comIconfont/iconfont/iconfont.json'
	export default {
		name: 'module',
		components: {
			Sticky,
			permissionBtn,
			Treeselect,
			treeTable,
			Pagination
		},
		mixins: [extend],
		directives: {
			waves,
			elDragDialog
		},
		data() {
			return {
				selectedModuleRowId: null,
				iconData: iconData,
				normalizer(node) {
					// treeselect定义字段
					return {
						label: node.name,
						id: node.id,
						children: node.children
					}
				},
				moduleOptions: [],
				multipleSelection: [],
				selectMenus: [], // 菜单列表选中的值
				tableKey: 0,
				list: [], // 菜单列表
				total: 0,
				currentModule: null, // 左边模块treetable当前选中的项
				listLoading: true,
				listQuery: {
					// 查询条件
					page: 1,
					limit: 20,
					orgId: '',
					key: undefined
				},
				apps: [],

				showDescription: false,
				modules: [], // 用户可访问到的模块列表
				modulesTree: [], // 用户可访问到的所有模块组成的树
				temp: {
					// 模块临时值
					id: undefined,
					cascadeId: '',
					url: '',
					code: '',
					sortNo: 0,
					iconName: '',
					parentId: null,
					name: '',
					status: 0,
					isSys: false
				},
				dialogFormVisible: false, // 模块编辑框
				dialogStatus: '',
				dialogMenuVisible: false, // 菜单编辑框
				dialogMenuStatus: '',

				chkRoot: false, // 根节点是否选中
				treeDisabled: false, // 树选择框时候可用
				textMap: {
					update: '编辑',
					create: '添加'
				},
				rules: {
					name: [{
						required: true,
						message: '名称不能为空',
						trigger: 'blur'
					}]
				},
				columns: [
				  // treetable的列名
				  {
				    text: '模块名称',
				    value: 'name'
				  },
				  {
				    text: '模块标识',
				    value: 'code'
				  },
				  {
				    text: 'URL',
				    value: 'url'
				  }
				],
				downloadLoading: false
			}
		},
		computed: {
			isRoot: {
				get() {
					return this.chkRoot
				},
				set(v) {
					this.chkRoot = v
					if (v) {
						this.temp.parentName = '根节点'
						this.temp.parentId = null
					}
				}
			},
			modulesTreeRoot() {
				const root = [{
					name: '根节点',
					parentId: '',
					id: ''
				}]
				return root.concat(this.modulesTree)
			},

			dpSelectModule: {
				// 模块编辑框下拉选中的模块
				get: function() {
					if (this.dialogStatus === 'update') {
						return this.temp.parentId || ''
					} else {
						return ''
					}
				},
				set: function(v) {
					console.log('set org:' + v)
					if (v === undefined || v === null || !v) {
						// 如果是根节点
						this.temp.parentName = '根节点'
						this.temp.parentId = null
						return
					}
					this.temp.parentId = v
					var parentname = this.modules.find(val => {
						return v === val.id
					}).name
					this.temp.parentName = parentname
				}
			}
		},
		filters: {
			statusFilter(status) {
				const statusMap = {
					0: 'info',
					1: 'danger'
				}
				return statusMap[status]
			}
		},
		created() {

		},
		mounted() {
				this.getModules()
			// this.getModulesTree()
		},
		methods: {
			handleChangeTempIcon(item) {
				this.temp.iconName = item.font_class
			},
			handleChangeMenuTempIcon(item) {
				this.menuTemp.icon = item.font_class
			},
			rowClick(row, column) {
				if (column && column.type === 'selection') return
				const table = this.$refs.mainTable
				if (!table) return

				const isSameRow = this.selectedModuleRowId === row.id
				table.clearSelection()
				if (isSameRow) {
					this.selectedModuleRowId = null
					return
				}
				table.toggleRowSelection(row, true)
				this.selectedModuleRowId = row.id
			},

			handleSelectionChange(val) {
				this.multipleSelection = val;
				if (val.length === 1) {
					this.selectedModuleRowId = val[0].id
				} else if (val.length === 0) {
					this.selectedModuleRowId = null
				} else {
					this.selectedModuleRowId = null
				}
			},

			onBtnClicked: function(domId) {
				console.log('you click:' + domId)
				switch (domId) {
					case 'btnAdd':
						this.handleCreate()
						break
					case 'btnEdit':
						if (this.currentModule === null) {
							this.$message({
								message: '只能选中一个进行编辑',
								type: 'error'
							})
							return
						}
						this.handleUpdate(this.currentModule)
						break
					case 'btnDel':
						if (this.currentModule === null) {
							this.$message({
								message: '至少删除一个',
								type: 'error'
							})
							return
						}
						this.handleDelete(this.currentModule)
						break
					default:
						break
				}
			},

			getModules() {
				this.listLoading = true;

				var _this = this // 记录vuecomponent
				modules.getList().then(response => {
					console.log(response)
					_this.list = response.result;
					_this.listLoading = false;
					
					_this.gateways = response.result.map(function(item, index, input) {
						return {
							key: item.id,
							display_name: item.name
						};
					});
					_this.moduleOptions = JSON.parse(JSON.stringify(_this.gateways));
					
				})
			},
			getModulesTree(){
			var _this = this // 记录vuecomponent
			login.getModules().then(response => {
				
			  _this.modules = response.result.map(function(item) {
			    return {
			      sortNo: item.sortNo,
			      id: item.id,
			      name: item.name,
			      iconName: item.iconName,
			      parentId: item.parentId || null,
			      code: item.code,
			      url: item.url,
			      cascadeId: item.cascadeId,
			      isSys: item.isSys
			    }
			  })
			  var modulestmp = JSON.parse(JSON.stringify(_this.modules))
			  _this.modulesTree = listToTreeSelect(modulestmp).sort((a, b) => a.sortNo - b.sortNo)
			})
			},
			handleFilter() {
				this.listQuery.page = 1
				this.getList()
			},
			handleSizeChange(val) {
				this.listQuery.limit = val
				this.getList()
			},
			handleCurrentChange(val) {
				this.listQuery.page = val.page
				this.listQuery.limit = val.limit
				this.getList()
			},
			resetTemp() {
				this.temp = {
					id: undefined,
					cascadeId: '',
					url: '',
					iconName: '',
					code: '',
					parentId: null,
					name: '',
					status: 0
				}
			},
			resetMenuTemp() {
				this.menuTemp = {
					id: undefined,
					cascadeId: '',
					icon: '',
					url: '',
					code: '',
					moduleId: this.currentModule ? this.currentModule.id : null,
					name: '',
					status: 0,
					sort: 0
				}
			},

			// #region 模块管理
			handleCreate() {
				// 弹出添加框
				this.resetTemp()
				this.dialogStatus = 'create'
				this.dialogFormVisible = true
				this.dpSelectModule = null
				this.$nextTick(() => {
					this.$refs['dataForm'].clearValidate()
				})
			},
			createData() {
				// 保存提交
				this.$refs['dataForm'].validate(valid => {
					if (valid) {
						if (this.temp.url.indexOf('http') > -1 && !this.temp.code) {
							this.$message.error('请输入模块标识')
							return
						}
						modules.add(this.temp).then(response => {
							// 需要回填数据库生成的数据
							this.temp.id = response.result.id
							this.temp.cascadeId = response.result.cascadeId
							this.list.unshift(this.temp)
							this.dialogFormVisible = false
							this.$notify({
								title: '成功',
								message: '创建成功',
								type: 'success',
								duration: 2000
							})
							this.getModules()
						})
					}
				})
			},
			handleUpdate() {
				if (this.multipleSelection.length !== 1) {
					this.$message({
						message: "只能选中一个进行编辑",
						type: "error"
					});
					return;
				} else {
					var row = this.multipleSelection[0];
					// 弹出编辑框
					this.temp = Object.assign({}, row) // copy obj
					if (this.temp.children) { // 点击含有子节点树结构时，带有的children会造成提交的时候json死循环
						this.temp.children = null
					}

					this.dialogStatus = 'update'
					this.dialogFormVisible = true
					this.dpSelectModule = this.temp.parentId
					this.$nextTick(() => {
						this.$refs['dataForm'].clearValidate()
					})
				}
			},
			updateData() {
				// 更新提交
				this.$refs['dataForm'].validate(valid => {
					if (valid) {
						const tempData = Object.assign({}, this.temp)
						if (tempData.url.indexOf('http') > -1 && !tempData.code) {
							this.$message.error('请输入模块标识')
							return
						}
						modules.update(tempData).then(() => {
							this.dialogFormVisible = false
							this.$notify({
								title: '成功',
								message: '更新成功',
								type: 'success',
								duration: 2000
							})

							this.getModules()
							for (const v of this.list) {
								if (v.id === this.temp.id) {
									const index = this.list.indexOf(v)
									this.list.splice(index, 1, this.temp)
									break
								}
							}
						})
					}
				})
			},
			handleDelete(row) {
				if (this.multipleSelection.length < 1) {
					this.$message({
						message: "至少删除一个",
						type: "error"
					});
					return;
				}
				this.$confirm("确定要删除吗？")
					.then(_ => {
						var rows=this.multipleSelection;
						var selectids = rows.map(u => u.id);
						var param = {
							ids: selectids
						};
						modules.del(param).then(() => {
							this.$notify({
								title: "成功",
								message: "删除成功",
								type: "success",
								duration: 2000
							});
							rows.forEach(row => {
								const index = this.list.indexOf(row);
								this.list.splice(index, 1);
							});
						});
					})
					.catch(_ => {});
					
				
			},
			// #end region


		}
	}
</script>

<style lang="scss">
	.text {
		font-size: 14px;
	}

	.item {
		margin-bottom: 18px;
	}

	.clearfix:before,
	.clearfix:after {
		display: table;
		content: '';
	}

	.clearfix:after {
		clear: both;
	}

	.el-card__header {
		padding: 12px 20px;
	}

	.selectIcon-box {
		text-align: center;
		border: 1px solid #eeeeee;
		border-right: 0;
		border-bottom: 0;

		.el-col {
			padding: 10px 0;
			border-right: 1px solid #eeeeee;
			border-bottom: 1px solid #eeeeee;

			&.active {
				.iconfont {
					color: #409EFF;
				}
			}
		}

		.iconfont {
			cursor: pointer;
			font-size: 20px;
		}
	}

	.custom-icon-input::before {
		font-size: 18px;
		position: absolute;
		right: 10px;
		top: 0;
	}
</style>
