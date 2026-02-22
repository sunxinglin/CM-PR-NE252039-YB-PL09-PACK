<template>
    <div>
        <div class="app-container">
            <el-card shadow="never" class="boby-small" style="height: 100%">
                <div slot="header" class="clearfix">
                    <span>刮胶处理</span>
                </div>
                <div>
                    <el-row :gutter="2">
                        <!--<el-col :span="21">

                            <el-button type="primary" icon="el-icon-search" size="small" @click="getlist">查询</el-button>-->
                        <!-- <el-button type="primary"  icon="el-icon-edit" size="small" @click="handleUpdate">编辑</el-button> -->
                        <!--</el-col>-->
                        <el-col :span="21">
                            <el-input prefix-icon="el-icon-search" size="small" style="width: 200px;" class="filter-item" :placeholder="'下箱体码'" v-model="query.packPN"></el-input>
                            <el-button type="primary" icon="el-icon-search" size="small" style="margin-left:10px;" @click="loadData">查询</el-button>
                            <el-button type="danger" icon="el-icon-delete" size="small" @click="clearGlueData">清除涂胶信息</el-button>
                        </el-col>
                    </el-row>
                </div>
            </el-card>
        </div>
        <div class="app-container">

            <div style=" color: gray; background-color: white; padding: 10px;">任务记录</div>
            <el-table ref="mainList"
                      :data="mainListData"
                      v-loading="mainListLoading"
                      row-key="id"
                      style="width: 100%;"
                      border
                      fit
                      stripe
                      highlight-current-row
                      align="left"
                      :height="mainTableHeight">

                <el-table-column label="下箱体码" prop="packCode" align="center" min-width="40px" />
                <!--<el-table-column label="AGV" prop="useAGVCode" align="center" min-width="40px" />-->
                <el-table-column label="创建时间" prop="createTime" align="center" min-width="40px" />
                <el-table-column label="任务状态" prop="status" align="center" min-width="40px">
                    <template slot-scope="scope">
                        <span>
                            {{ scope.row.status == 0 ? "未开始" : scope.row.status == 1 ? "进行中" : scope.row.status == 2 ? "已完成" : "" }}
                        </span>
                    </template>
                </el-table-column>

            </el-table>

            <div style=" color: gray; background-color: white; padding: 10px;margin-top:15px;">涂胶数据</div>

            <el-table ref="gluingInfoList"
                      :data="gluingInfoListData"
                      v-loading="ListLoading"
                      row-key="id"
                      style="width: 100%;"
                      border
                      fit
                      stripe
                      highlight-current-row
                      align="left"
                      :height="tablegeight">

                <el-table-column label="下箱体码" prop="packPN" align="center" min-width="300px" />
                <el-table-column label="程序号" prop="programNo" align="center" min-width="40px" />
                <el-table-column label="参数名称" prop="parameterName" align="center" min-width="80px" />
                <el-table-column label="上传MES代码" prop="upMesCodePN" align="center" min-width="60px" />

                <el-table-column label="参数值" prop="upValue" align="center" min-width="100px" />
                <el-table-column label="创建时间" prop="createTime" align="center" min-width="100px" />



            </el-table>


            <!--<el-table ref="detaillist"
                      :data="detetaillist"
                      v-loading="ListLoading"
                      row-key="id"
                      style="width: 100%;"
                      border
                      fit
                      stripe
                      highlight-current-row
                      align="left"
                      :height="tablegeight">

                <el-table-column label="平台编号" prop="lineNo" align="center" min-width="40px" />
                <el-table-column label="库位" prop="locationNo" align="center" min-width="40px" />
                <el-table-column label="库位层级" prop="locationLevelNo" align="center" min-width="40px" />
                <el-table-column label="是否有模组" prop="hasModule" align="center" min-width="40px">
                    <template slot-scope="scope">
                        <span>
                            {{ scope.row.hasModule ? "是" : "否" }}
                        </span>
                    </template>
                </el-table-column>
                <el-table-column label="模组号" prop="moduleCode" align="center" min-width="150px" />
                <el-table-column label="模组类型" prop="moduleType" align="center" min-width="40px" />
                <el-table-column label="是否可用" prop="isEnable" align="center" min-width="40px">
                    <template slot-scope="scope">
                        <span>
                            {{ scope.row.isEnable ? "是" : "否" }}
                        </span>
                    </template>
                </el-table-column>
                <el-table-column label="是否繁忙" prop="isBusy" align="center" min-width="40px">
                    <template slot-scope="scope">
                        <span>
                            {{ scope.row.hasUpMesDone ? "是" : "否" }}
                        </span>
                    </template>
                </el-table-column>

            </el-table>-->
        </div>

    </div>
</template>
<script>
    import waves from "@/directive/waves"; // 水波纹指令
    import Sticky from "@/components/Sticky";
    import permissionBtn from "@/components/PermissionBtn";
    import Pagination from "@/components/Pagination";
    import elDragDialog from "@/directive/el-dragDialog";
    import * as stationTaskMainAPI from "@/api/stationTaskMain";
    import * as glueDetailAPI from "@/api/gluedetail";
    export default {
        components: {
            Sticky,
            permissionBtn,
            Pagination,
        },
        directives: {
            waves,
            elDragDialog,
        },
        mounted() {
            let h = document.documentElement.clientHeight;
            let topH = this.$refs.mainList.$el.offsetTop;
            this.mainTableHeight = (h - topH) * 0.15;

            //this.tablegeight = (h - topH) * 0.3;
        },
        data() {
            return {
                query: {
                    packPN: "",
                    stepId: 3
                },
                mainListLoading: false,
                mainListData: [],
                mainTableHeight: null,

                ListLoading: false,
                gluingInfoListData: [],
                tablegeight: null,
            };
        },
        methods: {
            loadData() {
                if (!this.query.packPN) {
                    this.$message({ message: '请输入Pack码', type: "error", });
                    return;
                }
                this.getStationTaskMainList();
                this.getGluingInfoList();
            },
            getStationTaskMainList() {
                this.mainListLoading = true;
                stationTaskMainAPI.GetList(this.query).then((response) => {
                    this.mainListData = response.data;
                    this.mainListLoading = false;
                });
            },
            getGluingInfoList() {
                this.ListLoading = true;
                glueDetailAPI.getRealTimeGlueList({ PackPN: this.query.packPN, page: 1, limit: 100 }).then((response) => {
                    this.gluingInfoListData = response.data; //提取数据表
                    this.ListLoading = false;
                });
            },
            clearGlueData() {
                console.log('click clear glue data btn');

                if (this.mainListData.length <= 0 && this.gluingInfoListData.length <= 0) {
                    this.$message({ message: '暂无可清除的数据', type: "error", });
                    return;
                }

                this.$confirm('确定要清除吗？操作后不可恢复').then(_ => {

                    let mainIdArr = [];
                    this.mainListData.forEach((item) => { mainIdArr.push(item.id); });
                    console.log(mainIdArr);

                    let gluingInfoIdArr = [];
                    this.gluingInfoListData.forEach((item) => { gluingInfoIdArr.push(item.id); });
                    console.log(gluingInfoIdArr);


                    glueDetailAPI.deleteGluingInfo({ MainIds: mainIdArr, GluingInfoIds: gluingInfoIdArr }).then((response) => {
                        this.loadData();
                    }).catch(_ => { });
                }).catch(() => { });
            }
        },
    };
</script>