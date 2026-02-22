<template>
	<div>
		<sticky :className="'sub-navbar'">
			<div class="filter-container">
				<el-select ref="selarea" clearable filterable size="mini" class="filter-item" style="width: 200px"
					v-model="listQuery.areaId" :placeholder="'选择库区'">
					<el-option value label>请选择</el-option>
					<el-option v-for="item in  com_areaOptions" :key="item.key" :label="item.display_name"
						:value="item.key"></el-option>
				</el-select>
				<el-input @keyup.enter.native="handleFilter" size="mini"
					style="width: 200px; margin-bottom: 0; margin-left: 20px; margin-right: 20px;" class="filter-item"
					:placeholder="'库位编号'" v-model="listQuery.key">
				</el-input>
				<el-button type="primary" size="mini" @click="handleFilter">查询</el-button>
			</div>
		</sticky>
		<div class="app-container">

			<div class="bg-white">
				<el-table ref="mainTable" :key='tableKey' :data="list" v-loading="listLoading" row-key="id" border fit
					stripe highlight-current-row style="width: 100%;" @row-click="rowClick">
					<el-table-column :show-overflow-tooltip="true" min-width="120px" :label="'编号层排列'" prop="code"
						sortable>
						<template slot-scope="scope">
							<span>{{scope.row.code}}</span>
						</template>
					</el-table-column>

					<el-table-column :show-overflow-tooltip="true" min-width="120px" :label="'所属仓库'" prop="warehouseId"
						sortable>
						<template slot-scope="scope">
							<span>{{scope.row.warehouse.name}}</span>
						</template>
					</el-table-column>
					<el-table-column :show-overflow-tooltip="true" min-width="120px" :label="'所属库区'" prop="areaId"
						sortable>
						<template slot-scope="scope">
							<span>{{scope.row.area.name}}</span>
						</template>
					</el-table-column>
					<el-table-column :show-overflow-tooltip="true" min-width="80px" :label="'长度'" prop="length"
						sortable>
						<template slot-scope="scope">
							<span>{{scope.row.length}}</span>
						</template>
					</el-table-column>
					<el-table-column :show-overflow-tooltip="true" min-width="80px" :label="'宽度'" prop="width" sortable>
						<template slot-scope="scope">
							<span>{{scope.row.width}}</span>
						</template>
					</el-table-column>
					<el-table-column :show-overflow-tooltip="true" min-width="80px" :label="'高度'" prop="height"
						sortable>
						<template slot-scope="scope">
							<span>{{scope.row.height}}</span>
						</template>
					</el-table-column>
					<el-table-column :show-overflow-tooltip="true" min-width="80px" :label="'载重'" prop="weight"
						sortable>
						<template slot-scope="scope">
							<span>{{scope.row.weight}}</span>
						</template>
					</el-table-column>
					<el-table-column :show-overflow-tooltip="true" min-width="80px" :label="'排号'" prop="lineNo"
						sortable>
						<template slot-scope="scope">
							<span>{{scope.row.lineNo}}</span>
						</template>
					</el-table-column>
					<el-table-column :show-overflow-tooltip="true" min-width="80px" :label="'列号'" prop="columnNo"
						sortable>
						<template slot-scope="scope">
							<span>{{scope.row.columnNo}}</span>
						</template>
					</el-table-column>
					<el-table-column :show-overflow-tooltip="true" min-width="80px" :label="'层号'" prop="floorNo"
						sortable>
						<template slot-scope="scope">
							<span>{{scope.row.floorNo}}</span>
						</template>
					</el-table-column>
					<el-table-column :show-overflow-tooltip="true" min-width="100px" :label="'库位类型'"
						prop="scope.row.locationType" sortable>
						<template slot-scope="scope">
							<span :class="statusClass(scope.row.locationType)">
								{{locationTypeOptions.find(u =>u.key ==
									scope.row.locationType).display_name}}
							</span>

						</template>
					</el-table-column>

				</el-table>

				<pagination v-show="total>0" :total="total" :page.sync="listQuery.page" :limit.sync="listQuery.limit"
					@pagination="handleCurrentChange" />
			</div>


		</div>
	</div>

</template>

<script>
	import {
		listToTreeSelect
	} from '@/utils'
	import * as locations from '@/api/wms/Location'
	import * as areas from "@/api/wms/Area";
	import waves from '@/directive/waves' // 水波纹指令
	import Sticky from '@/components/Sticky'
	import Pagination from '@/components/Pagination'
	import elDragDialog from '@/directive/el-dragDialog'

	export default {
		name: 'select-locations',
		components: {
			Sticky,
			Pagination
		},
		   props: ['isKuKou'],
		directives: {
			waves,
			elDragDialog
		},
		data() {
			return {
				com_areaOptions: [],
				locationTypeOptions: [{
						key: 0,
						display_name: "正常尺寸"
					},
					{
						key: 1,
						display_name: "超宽或超高"
					},
					{
						key: 2,
						display_name: "库口"
					}
				],
				statusOptions: [{
						key: 0,
						display_name: "未使用"
					},
					{
						key: 1,
						display_name: "已使用"
					},
					{
						key: 2,
						display_name: "锁定"
					}
				],
				multipleSelection: [], // 列表checkbox选中的值
				tableKey: 0,
				list: null,
				total: 0,
				listLoading: true,
				listQuery: { // 查询条件
					page: 1,
					limit: 10,
					key: undefined,
					status: 0,
					areaId: undefined,
					isDisabled: false,
					isKukou: this.isKuKou,

				},
				downloadLoading: false
			}
		},
		computed: {

		},
		filters: {
			statusFilter(status) {
				var res = 'color-success'
				switch (status) {
					case 1:
						res = 'color-danger'
						break
					default:
						break
				}
				return res
			}
		},
		created() {
			this.getArea()
			this.getList()
		},
		mounted() {},
		methods: {
			rowClick(row) {
				//				this.$refs.mainTable.clearSelection()
				//				this.$refs.mainTable.toggleRowSelection(row)
				this.$emit('locations-change', row)
			},

			handleSelectionChange(val) {
				this.multipleSelection = val
			},
			getList() {

				this.listLoading = true;
				locations.getPageList(this.listQuery).then(response => {
					this.list = response.data;
					this.total = response.count;
					this.listLoading = false;
				});
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
			getArea() {
				var _this = this; // 记录vuecomponent
				var param = {

				}
				areas.getAreaList(param).then(response => {
					_this.areas = response.result.map(function(item, index, input) {
						return {
							key: item.id,
							display_name: item.name
						};
					});
					_this.com_areaOptions = JSON.parse(JSON.stringify(_this.areas));
				});
			},
			statusClass(i) {
				//根据状态显示不同的样式
				switch (i) {
					case 0:
						return "statusA";
					case 1:
						return "statusB";
					case -1:
						return "statusC";
					case 2:
						return "statusE";
					default:
						return "statusD";
				}
			},


		}
	}
</script>
