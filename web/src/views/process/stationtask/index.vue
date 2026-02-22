<template>
	<div class="flex-column">
		<sticky :className="'sub-navbar'">
			<div class="filter-container">
				<el-button type="primary" size="small" icon="el-icon-xinzeng" @click="handleCreate">添加任务</el-button>
				<el-button type="primary" size="small" icon="el-icon-edit" @click="handleUpdate">任务详情</el-button>
				<el-button type="danger" size="small" icon="el-icon-delete" @click="handleDelete">删除</el-button>
			</div>
		</sticky>

		<div class="app-container flex-item">
			<div class="bg-white fh">

				<el-table ref="subTable" style="width: 100%" border fit stripe highlight-current-row align="left">
					<el-table-column prop="stdtask.code" label="编码" min-width="20px" sortable align="center">
					</el-table-column>
					<el-table-column prop="stdtask.name" label="名称" min-width="20px" sortable align="center">
					</el-table-column>
					<el-table-column prop="stdtask.type" label="类型" :formatter="setstationTaketype" min-width="20px"
						sortable align="center"></el-table-column>
					<el-table-column prop="stdtask.hasPage" label="是否有页面" :formatter="formatterBoolean" min-width="20px"
						sortable align="center"></el-table-column>
					<el-table-column prop="stdtask.sequence" label="操作顺序" min-width="20px" sortable align="center">
					</el-table-column>
					<el-table-column prop="stdtask.description" label="描述" min-width="20px" sortable align="center">
					</el-table-column>
				</el-table>
			</div>
		</div>
	</div>

</template>

<script>
	import {
		listToTreeSelect
	} from '@/utils'
	import * as accsssObjs from '@/api/accessObjs'
	import * as users from '@/api/users'
	import * as apiRoles from '@/api/roles'
	import waves from '@/directive/waves' // 水波纹指令
	import elDragDialog from '@/directive/el-dragDialog'
	import extend from "@/extensions/delRows.js"

	export default {
		name: "stationtask",
		components: {},
		mixins: [extend],
		directives: {
			waves,
			elDragDialog
		},
		data() {
			return {
				stationId: 0,
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
			this.getRouterData();
			this.getList();
		},
		mounted() {
			// var _this = this // 记录vuecomponent
			// login.getOrgs().then(response => {
			//   _this.orgs = response.result.map(function(item) {
			//     return {
			//       id: item.id,
			//       label: item.name,
			//       parentId: item.parentId || null
			//     }
			//   })
			//   var orgstmp = JSON.parse(JSON.stringify(_this.orgs))
			//   _this.orgsTree = listToTreeSelect(orgstmp)
			// })
		},
		methods: {
			getRouterData() {
				console.log('params', this.$route.query)
				this.stationId = this.$route.query.stationId;
				console.log('stationId', this.stationId)
			}
		}
	}
</script>

<style>
	.clearfix:before,
	.clearfix:after {
		display: table;
		content: "";
	}

	.clearfix:after {
		clear: both
	}

	.el-card__header {
		padding: 12px 20px;
	}

	.body-small.dialog-mini .el-dialog__body .el-form {
		padding-right: 0px;
		padding-top: 0px;
	}
</style>
