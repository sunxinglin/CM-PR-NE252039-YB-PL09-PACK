<template>
    <div>
        <div class="app-container">
            <el-card shadow="never" class="boby-small" style="height: 100%">
                <div slot="header" class="clearfix">
                    <span>下箱体涂胶详情数据</span>
                </div>
                <div>
                    <el-row :gutter="2">
                        <el-col :span="21">

                            <el-input prefix-icon="el-icon-search"
                                      size="small"
                                      style="width: 200px;height: 20px;"
                                      class="filter-item"
                                      :placeholder="'pack码/箱体码'"
                                      v-model="query.PackPN">
                            </el-input>

                            <el-date-picker clearable filterable size="small" class="filter-item" placeholder="开始日期" type="date"
                                            :editable="false" v-model="query.beginTime" value-format="yyyy-MM-dd" @change="changeDate"
                                            :picker-options="pickerOptions0">
                            </el-date-picker>

                            <el-date-picker ref="time" clearable filterable size="small" class="filter-item" placeholder="结束日期" type="date"
                                            :editable="true" v-model="query.endTime" @change="changeDate" :picker-options="pickerOptions1"
                                            value-format="yyyy-MM-dd">
                            </el-date-picker>

                            <el-button type="primary"
                                       icon="el-icon-plus"
                                       size="small"
                                       @click="getlist">
                                查询
                            </el-button>
                            <el-button type="primary" icon="el-icon-plus" size="small" @click="ModelExpornt">
                                导出
                            </el-button>
                            <el-button type="primary" icon="el-icon-plus" size="small" @click="upgluedata">
                                重传
                            </el-button>
                        </el-col>
                    </el-row>
                </div>
            </el-card>
        </div>
        <div class="app-container">
            <el-table ref="detaillist" :data="detetaillist" v-loading="ListLoading" row-key="id" style="width: 100%"
                      border fit stripe highlight-current-row align="left" :height="tablegeight">
                <el-table-column label="Pack编码" prop="packPN" align="center" width="300px" />
                <el-table-column label="任务名称" prop="parameterName" align="center" />
                <el-table-column label="上传代码" prop="upMesCodePN" align="center" />
                <el-table-column label="任务数据" prop="upValue" align="center" />
                <el-table-column label="创建日期" prop="createTime" align="center" />
            </el-table>

            <div>
                <pagination :total="total"
                            v-show="total > 0"
                            hide-on-single-page
                            :page.sync="query.page"
                            :limit.sync="query.limit"
                            @pagination="getlist" />
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
    import * as gluedetail from "@/api/lowerboxglue";
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
                    PackPN: "",
                    beginTime: null,
                    endTime: null,
                },
                tablegeight: null,
                total: 0,
                ListLoading: false,
                pickerOptions1: {},
                pickerOptions0: {},
         
             
            };
        },
        methods: {
            getlist() {
                this.ListLoading = true;
                gluedetail.Load(this.query).then((response) => {
                    this.detetaillist = response.data; //提取数据表
                    this.total = response.count; //提取数据表条数
                    console.log(this.total);
                    this.ListLoading = false;
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
                    if (
                        this.query.PackPN == null ||
                        this.query.PackPN.length <= 0
                    ) {
                        this.$message({
                        message: "请输入需要重传的PACK条码!",
                        type: "error",
                        });
                        return;
                    }
                    console.log("请求"+this.query);
                    automicstationdataupcatlagain
                    .upCatlAgain({ PackPN: this.query.PackPN, Code: "OP120" })
                    .then((response) => {
                        console.log("返回"+response);
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