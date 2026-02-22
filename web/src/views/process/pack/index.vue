<template><!-- 生产 -->
	<div>
		<div class="app-container">
			<el-row :gutter="4">
				<el-col :span="24">
					<el-card shadow="never" class="boby-small">
						<div slot="header" class="clearfix">
							<span>Pack</span>
						</div>
						<div style="margin-bottom: 10px;">
							<el-row :gutter="4">
								<el-col :span="22">
									<el-button type="primary" icon="el-icon-plus" size="mini" @click="handleCreate">添加</el-button>
									<el-button type="primary" icon="el-icon-edit" size="mini" @click="handleUpdate">编辑</el-button>
									<el-button type="primary" icon="el-icon-delete" size="mini" @click="handleDelete">删除</el-button>
								</el-col>
								<el-col :span="2">
									<el-input @keyup.enter.native="handleFilter" prefix-icon="el-icon-search" size="mini" style="width: 100px;"
									 class="filter-item" :placeholder="'关键字'" v-model="packListQuery.key"></el-input>
								</el-col>
							</el-row>
						</div>
						<div>
							<el-table ref="mainTable" :data="packList" v-loading="packListLoading" row-key="id" style="width: 100%;"
							 @selection-change="handleSelectionChange" border fit stripe highlight-current-row align="left">
								<el-table-column type="selection" min-width="20px" align="center"></el-table-column>
								<el-table-column prop="code" label="编号" min-width="20px" sortable align="center"></el-table-column>
								<el-table-column prop="name" label="名称" min-width="20px" sortable align="center"></el-table-column>
								<el-table-column prop="packSN" label="Pack条码" min-width="35px" sortable align="center"></el-table-column>
								<el-table-column prop="onlineTime" label="上线时间" min-width="40px" sortable align="center"></el-table-column>
								<el-table-column prop="isComplete" label="是否完成" min-width="30px" sortable align="center" :formatter="formatterBoolean"></el-table-column>
								<el-table-column prop="completeTime" label="完成时间" min-width="40px" sortable align="center"></el-table-column>
								<el-table-column prop="isNG" label="是否NG" min-width="30px" sortable align="center" :formatter="formatterBoolean"></el-table-column>
								<el-table-column prop="currentStation" label="当前工位" min-width="30px" sortable align="center"></el-table-column>
								<el-table-column prop="isUploadMES" label="是否上传MES" min-width="35px" sortable align="center" :formatter="formatterBoolean"></el-table-column>
								<el-table-column prop="description" label="描述" min-width="20px" sortable align="center"></el-table-column>
							</el-table>
						</div>
						<div>
							<pagination :total="packTotal" v-show="packTotal>0" :page.sync="packListQuery.page" :limit.sync="packListQuery.limit"
							 @pagination="handleCurrentChange" />
						</div>
					</el-card>
				</el-col>
			</el-row>
			<el-dialog v-el-drag-dialog class="dialog-mini" width="500px" :title="textMap[dialogStatus]" :visible.sync="dialogFormVisible">
				<div>
					<el-form :rules="packRules" ref="dataForm" :model="packTemp" label-position="right" label-width="100px">
						<el-form-item size="small" :label="'编号'" prop="code">
							<el-input v-model="packTemp.code"></el-input>
						</el-form-item>
						<el-form-item size="small" :label="'名称'" prop="name">
							<el-input v-model="packTemp.name"></el-input>
						</el-form-item>
						<el-form-item size="small" :label="'Pack条码'" prop="packSN">
							<el-input v-model="packTemp.packSN"></el-input>
						</el-form-item>
						<el-form-item size="small" :label="'上线时间'" prop="onlineTime">
							<el-date-picker type="datetime" v-model="packTemp.onlineTime" value-format="yyyy MM dd HH:mm:ss"></el-date-picker>
						</el-form-item>
						<el-form-item size="small" :label="'是否完成'" prop="isComplete">
							<el-switch v-model="packTemp.isComplete"></el-switch>
						</el-form-item>
						<el-form-item size="small" :label="'完成时间'" prop="completeTime">
							<el-date-picker type="datetime" v-model="packTemp.completeTime" value-format="yyyy MM dd HH:mm:ss"></el-date-picker>
						</el-form-item>
						<el-form-item size="small" :label="'是否NG'" prop="isNG">
							<el-switch v-model="packTemp.isNG"></el-switch>
						</el-form-item>
						<el-form-item size="small" :label="'当前工位'" prop="currentStation">
							<el-input v-model="packTemp.currentStation"></el-input>
						</el-form-item>
						<el-form-item size="small" :label="'是否上传MES'" prop="isUploadMES">
							<el-switch v-model="packTemp.isUploadMES"></el-switch>
						</el-form-item>
						<el-form-item size="small" :label="'描述'">
							<el-input v-model="packTemp.description" :min="1" :max="20"></el-input>
						</el-form-item>
					</el-form>
				</div>
				<div slot="footer">
					<el-button size="mini" @click="dialogFormVisible=false">取消</el-button>
					<el-button size="mini" v-if="dialogStatus=='create'" type="primary" @click="createData">确认</el-button>
					<el-button size="mini" v-else type="primary" @click="updateData">确认</el-button>
				</div>
			</el-dialog>
		</div>
	</div>
</template>

<script>
	import * as packs from "@/api/pack";
	import * as axios from "axios";
	import waves from "@/directive/waves"; // 水波纹指令
	import Sticky from "@/components/Sticky";
	import permissionBtn from "@/components/PermissionBtn";
	import Pagination from "@/components/Pagination";
	import elDragDialog from "@/directive/el-dragDialog";
	export default {
		name: 'pack',

		components: {
			Sticky,
			permissionBtn,
			Pagination
		},
		directives: {
			waves,
			elDragDialog
		},
		data() {
			return {
				packMultipleSelection: [], //勾选的数据表值
				packList: [], //数据表
				packTotal: 0, //数据条数
				packListLoading: true, //加载特效
				packListQuery: { //查询条件
					page: 1,
					limit: 20,
					key: undefined
				},
				packTemp: {
					//模块临时值
					id: undefined,
					code: '',
					name: '',
					packSN: '',
					onlineTime: '',
					isComplete: false,
					completeTime: '',
					isNG: false,
					currentStation: '',
					isUploadMES: false,
					description: ''
				},
				dialogFormVisible: false, //编辑框
				dialogStatus: '', //编辑框功能(添加/编辑)
				textMap: {
					update: '编辑',
					create: '添加'
				},
				packRules: { //编辑框输入限制
					code: [{
						required: true,
						message: '编号不能为空',
						trigger: 'blur'
					}],
					name: [{
						required: true,
						message: '名称不能为空',
						trigger: 'blur'
					}],
					packSN: [{
						required: true,
						message: 'pack条码不能为空',
						trigger: 'blur'
					}],
					onlineTime: [{
						required: true,
						message: ' 上线时间不能为空',
						trigger: 'blur'
					}],
					currentStation: [{
						required: true,
						message: '当前工位不能为空',
						trigger: 'blur'
					}]
				},
			}
		},
		mounted() {
			this.packLoad();
		},
		methods: {
			//Bool转换
			formatterBoolean: function(row, column, cellValue) {
				var ret = ''
				if (cellValue) {
					ret = '是'
				} else {
					ret = '否'
				}
				return ret;
			},
			//勾选框
			handleSelectionChange(val) {
				this.packMultipleSelection = val;
			},
			//关键字搜索
			handleFilter() {
				this.packLoad();
			},
			//分页
			handleCurrentChange(val) {
				this.packListQuery.page = val.page;
				this.packListQuery.limit = val.limit;
				this.packLoad(); //页面加载
			},
			//列表加载
			packLoad() {
				this.packListLoading = true;
				packs.load(this.packListQuery).then(response => {
					this.packList = response.data; //提取数据表
					this.packTotal = response.count; //提取数据表条数
					this.packListLoading = false;
				})
			},
			//编辑框数值初始值
			resetTemp() {
				this.packTemp = {
					id: undefined,
					code: '',
					name: '',
					packSN: '',
					onlineTime: '',
					isComplete: true,
					completeTime: '',
					isNG: false,
					currentStation: '',
					isUploadMES: false,
					description: ''
				}
			},
			//点击添加
			handleCreate() {
				//弹出编辑框
				this.resetTemp(); //数值初始化
				this.dialogStatus = 'create'; //编辑框功能选择（添加）
				this.dialogFormVisible = true; //编辑框显示
				this.$nextTick(() => {
					this.$refs['dataForm'].clearValidate();
				});
			},
			//保存提交
			createData() {
				this.$refs['dataForm'].validate(valid => {
					if (valid) {
						packs.add(this.packTemp).then(response => {
							this.dialogFormVisible = false; //编辑框关闭
							this.$notify({
								title: '成功',
								message: '创建成功',
								type: 'success',
								duration: 2000
							});
							this.packLoad(); //页面加载
						});
					}
				});
			},
			//点击编辑
			handleUpdate() {
				if (this.packMultipleSelection.length !== 1) {
					this.$message({
						message: '只能选中一个进行编辑',
						type: 'error'
					});
					return;
				} else {
					var row = this.packMultipleSelection[0];
					//弹出编辑框
					this.packTemp = Object.assign({}, row); //复制选中的数据
					this.dialogStatus = 'update'; //编辑框功能选择（更新）
					this.dialogFormVisible = true; //编辑框显示
					this.$nextTick(() => {
						this.$refs['dataForm'].clearValidate();
					});
				}
			},
			//更新提交
			updateData() {
				this.$refs['dataForm'].validate(valid => {
					if (valid) {
						packs.update(this.packTemp).then(() => {
							this.dialogFormVisible = false //编辑框关闭
							this.$notify({
								title: '成功',
								message: '更新成功',
								type: 'success',
								duration: 2000
							})
							this.packLoad(); //页面加载
						})
					}
				});
			},
			//点击删除
			handleDelete(row) {
				if (this.packMultipleSelection.length < 1) {
					this.$message({
						message: "至少删除一个",
						type: "error"
					});
					return;
				}
				this.$confirm("确定要删除吗？")
					.then(_ => {
						var rows = this.packMultipleSelection;
						var selectids = rows.map(u => u.id); //提取复选框的数据的Id
						var param = {
							ids: selectids
						};
						packs.del(param).then(() => {
							this.$notify({
								title: "成功",
								message: "删除成功",
								type: "success",
								duration: 2000
							});
							this.packLoad(); //页面加载
						});
					})
					.catch(_ => {});
			},
		}
	}
</script>
