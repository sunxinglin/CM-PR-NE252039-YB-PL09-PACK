<template>
    <div>
        <div class="app-container">
            <el-card shadow="never" class="boby-small" style="height: 100%">
                <div slot="header" class="clearfix">
                    <span>拧紧详情数据</span>
                </div>
                <div>
                    <el-row :gutter="2">
                        <el-col :span="21">
                            <el-input prefix-icon="el-icon-search" size="small" style="width: 200px;height: 20px;"
                                class="filter-item" :placeholder="'Pack编码'" v-model="query.PackPN">
                            </el-input>
                            <el-input prefix-icon="el-icon-search" size="small" style="width: 200px; height: 20px"
                                class="filter-item" :placeholder="'工序编码'" v-model="query.stationCode">
                            </el-input>
                            <el-select class="filter-item" size="small" v-model="query.resultIsOK" clearable placeholder="拧紧结果">
                                <el-option v-for="item in logtype" :key="item.key" :label="item.lable"
                                    :value="item.key">
                                </el-option>
                            </el-select>
                            <el-date-picker clearable filterable size="small" class="filter-item" placeholder="开始日期"
                                type="date" :editable="false" v-model="query.beginTime" value-format="yyyy-MM-dd"
                                @change="changeDate" :picker-options="pickerOptions0"></el-date-picker>

                            <el-date-picker clearable filterable size="small" class="filter-item" placeholder="结束日期"
                                type="date" :editable="false" v-model="query.endTime" @change="changeDate"
                                :picker-options="pickerOptions1" value-format="yyyy-MM-dd">
                            </el-date-picker>

                            <el-button type="infor" size="small" icon="el-icon-search" @click="handleFilter">
                                查询
                            </el-button>
                            <el-button type="primary" icon="el-icon-plus" size="small" @click="ModelExpornt">
                                导出
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
                <el-table-column label="工序名称"
                    prop="proc_StationTask_BlotGun.stationTask_Record.proc_StationTask_Main.step.code" align="center" />
                <el-table-column label="工位名称"
                    prop="proc_StationTask_BlotGun.stationTask_Record.proc_StationTask_Main.station.code"
                    align="center" />
                <el-table-column label="螺丝号" prop="orderNo" align="center" width="80px" />
                <el-table-column label="程序号" prop="programNo" align="center" width="80px" />
                <el-table-column label="扭力上传代码" prop="uploadCode" align="center" />
                <el-table-column label="角度上传代码" prop="uploadCode_JD" align="center" />
                <el-table-column label="拧紧结果" prop="resultIsOK" align="center">
                    <template slot-scope="scope">
                        <span>
                            {{ scope.row.resultIsOK ? "OK" : "NG" }}
                        </span>
                    </template>
                </el-table-column>

                <el-table-column label="角度值" prop="finalAngle" align="center" />
                <el-table-column label="扭矩值" prop="finalTorque" align="center" />

                <el-table-column label="日期" prop="createTime" align="center" />
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
import * as blotgundetail from "@/api/blotgundetail";
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
    },
    data() {
        return {
            detetaillist: [],
            logtype: [
                {
                    key: 1,
                    lable: "NG",
                },
                {
                    key: 2,
                    lable: "OK",
                }
            ],
            query: {
                page: 1,
                limit: 20,
                programNo: "",
                deviceNo: "",
                resultIsOK: null,
                PackPN: "",
                beginTime: null,
                endTime: null,
                stationCode: "",
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
            blotgundetail.getList(this.query).then((response) => {
                this.detetaillist = response.data; //提取数据表
                this.total = response.count; //提取数据表条数
                console.log(this.total);
                this.ListLoading = false;
            });
        },
        handleFilter() {
            this.query.page = 1;
            this.getlist();
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
            blotgundetail
                .modelExpornt(this.query)
                .then((response) => {
                    this.$notify({
                        title: "提示",
                        message: "数据整理完毕,正在下载",
                        type: "success",
                        duration: 2000,
                    });
                    window.location.href = `/api/StationTask_BlotGunDetail/DownloadExcel/${response.result}`
                });
        },
    },
};
</script>