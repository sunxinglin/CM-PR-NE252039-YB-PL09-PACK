<template>
    <div>
        <div class="app-container">
            <el-card shadow="never" class="boby-small" style="height: 100%">
                <div slot="header" class="clearfix">
                    <span>下箱体涂胶(新)</span>
                </div>
                <div>
                    <el-row :gutter="2">
                        <el-col :span="21">

                            <el-input prefix-icon="el-icon-search" size="small" style="width: 200px;height: 20px;"
                                class="filter-item" :placeholder="'pack码/箱体码'" v-model="query.PackCode">
                            </el-input>

                            <el-date-picker clearable filterable size="small" class="filter-item" placeholder="开始日期"
                                type="date" :editable="false" v-model="query.beginTime" value-format="yyyy-MM-dd"
                                @change="changeDate" :picker-options="pickerOptions0">
                            </el-date-picker>

                            <el-date-picker ref="time" clearable filterable size="small" class="filter-item"
                                placeholder="结束日期" type="date" :editable="true" v-model="query.endTime"
                                @change="changeDate" :picker-options="pickerOptions1" value-format="yyyy-MM-dd">
                            </el-date-picker>

                            <el-button type="primary" size="small" @click="sercchData" style="margin-left: 10px;">
                                查询
                            </el-button>
                            <el-button type="primary" size="small" @click="ModelExpornt">
                                导出
                            </el-button>
                            <el-button type="danger" size="small" @click="upgluedata">
                                重传
                            </el-button>
                        </el-col>
                    </el-row>
                </div>
            </el-card>
        </div>

        <div class="app-container">

            <el-table ref="timelist" :data="deteTimetaillist" v-loading="timeListLoading" row-key="id"
                style="width: 100%" border fit stripe highlight-current-row align="left" :height="100">
                <el-table-column label="Pack码" prop="serialCode" align="center" />
                <el-table-column label="工位" prop="stationCode" align="center" />
                <el-table-column label="上传代码" prop="uploadMesCode" align="center" />
                <el-table-column label="涂胶时间" prop="timeValue" align="center" />
                 <!-- <el-table-column label="" align="center">
          <template slot-scope="scope">
            <el-button size="mini" @click="ViewTaskDetails(scope.row)" type="danger">修改时间</el-button>
          </template>
        </el-table-column> -->
            </el-table>

            <el-table ref="detaillist" :data="detetaillist" v-loading="ListLoading" row-key="id" style="width: 100%"
                border fit stripe highlight-current-row align="left" :height="tablegeight">
                <el-table-column label="Pack码" prop="packPN" align="center" />
                <el-table-column label="工位" prop="stationCode" align="center" />
                <el-table-column label="参数" prop="paramName" align="center" />
                <el-table-column label="上传代码" prop="uploadMesCode" align="center" />
                <el-table-column label="数据" prop="value" align="center" />
                <el-table-column label="创建日期" prop="createTime" align="center" />
            </el-table>

        

            <div>
                <pagination :total="total" v-show="total > 0" hide-on-single-page :page.sync="query.page"
                    :limit.sync="query.limit" @pagination="getlist" />
            </div>
        </div>


    </div>
</template>
<script>
import waves from "@/directive/waves"; // 水波纹指令
import Sticky from "@/components/Sticky";
import permissionBtn from "@/components/PermissionBtn";
import Pagination from "@/components/Pagination";
import elDragDialog from "@/directive/el-dragDialog";
import * as gluedetail from "@/api/lowerboxgluenew";
import * as automicstationdataupcatlagain from "@/api/automicstationdataupcatlagain";
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
        let topH = this.$refs.detaillist.$el.offsetTop;
        this.tablegeight = (h - topH) * 0.81;
        //this.handleClickClose();
    },
    data() {
        return {
            detetaillist: [],
            query: {
                page: 1,
                limit: 20,
                PackCode: "",
                beginTime: null,
                endTime: null,
            },
            tablegeight: null,
            total: 0,
            ListLoading: false,
            pickerOptions1: {},
            pickerOptions0: {},

            deteTimetaillist: [],
            timeListLoading: false,

        };
    },
    methods: {

        sercchData() {

            if (!this.query.PackCode) {
                this.detetaillist = [];
                this.deteTimetaillist = [];
                this.$message({ message: "请输入需要查询的PACK条码!", type: "error", });
                return;
            }

            this.getlist();
            this.getTimelist();
        },
        getlist() {
            if (!this.query.PackCode) {
                return;
            }


            this.ListLoading = true;
            gluedetail.Load(this.query).then((response) => {
                this.detetaillist = response.result; //提取数据表
                this.total = response.count; //提取数据表条数
                console.log(this.total);
                this.ListLoading = false;
            });
        },
        getTimelist() {
            if (!this.query.PackCode) {
                return;
            }

            this.timeListLoading = true;
            gluedetail.TimeLoad(this.query).then((response) => {
                this.deteTimetaillist = response.result; //提取数据表
                this.total = response.count; //提取数据表条数
                console.log(this.total);
                this.timeListLoading = false;
            });
        },
        changeDate() {
            // debugger
            //因为date1和date2格式为 年-月-日， 所以这里先把date1和date2转换为时间戳再进行比较
            let date1 = new Date(this.query.beginTime).getTime();
            let date2 = new Date(this.query.endTime).getTime();

            this.pickerOptions0 = {
                disabledDate: (time) => {
                    if (date2 != "") {
                        return time.getTime() >= date2;
                    }
                },
            };
            this.pickerOptions1 = {
                disabledDate: (time) => {
                    return time.getTime() <= date1;
                },
            };
        },
        ModelExpornt() {
            gluedetail
                .modelExpornt(this.query)
                .then((request) => {
                    this.$notify({
                        title: "提示",
                        message: "数据整理完毕,正在下载",
                        type: "success",
                        duration: 2000,
                    });
                    var blob = new Blob([request], {
                        type: "application/vnd.ms-excel",
                    });
                    var fileName = "下箱体涂胶数据导出.xlsx";
                    if (window.navigator.msSaveOrOpenBlob) {
                        navigator.msSaveBlob(blob, fileName);
                    } else {
                        var link = document.createElement("a");

                        link.href = window.URL.createObjectURL(blob);
                        link.download = fileName;
                        link.click();
                        window.URL.revokeObjectURL(link.href);
                    }
                });
        },
        upgluedata() {
             if (!this.query.PackCode) {
                this.detetaillist = [];
                this.deteTimetaillist = [];
                this.$message({ message: "请输入需要查询的PACK条码!", type: "error", });
                return;
            }
            console.log("请求" + this.query);
            gluedetail.UploadDataAgain({ PackCode: this.query.PackCode, StationCode: "OP080" })
                .then((response) => {
                    console.log("返回" + response);
                    if (response.isError) {
                        this.$message({
                            message: "错误代码:" + response.errorCode + "\r\n" + "错误信息:" + response.errorMessage,
                            type: "error",
                        });
                        return;
                    }
                    this.$message({
                        message: "CATLMES返回正常",
                        type: "success",
                    });
                    return;

                });
        },

    },
};
</script>