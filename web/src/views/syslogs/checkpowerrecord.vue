<template>
    <div>
        <div class="app-container">
            <el-card shadow="never" class="boby-small" style="height: 100%">
                <div slot="header" class="clearfix">
                    <span>权限刷卡记录</span>
                </div>
                <div>
                    <el-row :gutter="2">
                        <el-col :span="21">

                            <el-date-picker clearable filterable size="small" class="filter-item" placeholder="开始日期" type="date"
                                            :editable="false" v-model="query.beginDate" value-format="yyyy-MM-dd" @change="changeDate"
                                            :picker-options="pickerOptions0"></el-date-picker>

                            <el-date-picker clearable filterable size="small" class="filter-item" placeholder="结束日期" type="date"
                                            :editable="false" v-model="query.endDate" @change="changeDate" :picker-options="pickerOptions1"
                                            value-format="yyyy-MM-dd">
                            </el-date-picker>

                            <el-select class="filter-item" size="small" v-model="query.moduleName" placeholder="请选择功能模块">
                                <el-option v-for="item in moduleType" :key="item.key" :label="item.lable" :value="item.key">
                                </el-option>
                            </el-select>

                            <el-input :placeholder="'工位'"
                                      size="small"
                                      style="width: 120px"
                                      class="filter-item"
                                      prefix-icon="el-icon-search"
                                      v-model="query.StationCode"></el-input>

                            <el-input :placeholder="'PACK编码'"
                                      size="small"
                                      style="width: 300px"
                                      class="filter-item"
                                      prefix-icon="el-icon-search"
                                      v-model="query.PackCode"></el-input>

                            <!--<el-input prefix-icon="el-icon-search" size="small" style="width: 200px;height: 36px;" class="filter-item" :placeholder="'AGV'" v-model="query.agvNo"></el-input>
    <el-input size="small" style="width: 200px;height: 20px;" class="filter-item" :placeholder="'下箱体码'" v-model="query.outerGoodsCode"></el-input>-->
                            <el-button type="primary" icon="el-icon-search" size="small" @click="getList">查询</el-button>
                        </el-col>
                    </el-row>
                </div>
            </el-card>
        </div>
        <div class="app-container">
            <el-table ref="detaillist"
                      :data="detetaillist"
                      v-loading="ListLoading"
                      row-key="id"
                      style="width: 100%"
                      border
                      fit
                      stripe
                      highlight-current-row
                      align="left"
                      :height="tablegeight">
                <el-table-column label="模块" prop="moduleName" align="center" min-width="30px" />
                <el-table-column label="工位" prop="stationCode" align="center" min-width="30px" />
                <el-table-column label="Pack码" prop="packCode" align="left" min-width="70px" />
                <el-table-column label="帐号" prop="accountName" align="center" min-width="30px" />
                <el-table-column label="报警信息" prop="alarm" align="left" min-width="300px" />
                <el-table-column label="发生时间" prop="createTime" align="center" min-width="60px" />

            </el-table>

            <div>
                <pagination :total="total"
                            v-show="total > 0"
                            hide-on-single-page
                            :page.sync="query.page"
                            :limit.sync="query.limit"
                            @pagination="getList" />
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
    import * as checkPowerRecordAPI from "@/api/checkpowerrecord";

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

            this.changeDate();
        },
        data() {
            return {
                detetaillist: [],
                query: {
                    page: 1,
                    limit: 10,
                    moduleName: "",
                    beginDate: new Date(),
                    endDate: new Date(),
                    PackCode: '',
                    StationCode:''
                },
                tablegeight: null,
                total: 0,
                ListLoading: false,
                moduleType: [
                    {
                        key: "全部",
                        lable: "全部",
                    },
                    {
                        key: "返工设置",
                        lable: "返工设置",
                    },
                    {
                        key: "拧紧NG复位",
                        lable: "拧紧NG复位",
                    },
                    {
                        key: "参数设置",
                        lable: "参数设置",
                    },
                    {
                        key: "调试工具",
                        lable: "调试工具",
                    },
                    {
                        key: "AGV",
                        lable: "AGV",
                    }
                ],
                pickerOptions1: {},
                pickerOptions0: {},
            };
        },
        methods: {
            getList() {
                this.ListLoading = true;
                checkPowerRecordAPI.GetPageList(this.query).then((response) => {
                    this.detetaillist = response.data; //提取数据表
                    this.total = response.count; //提取数据表条数
                    this.ListLoading = false;
                });
            },
            changeDate() {
                // debugger
                //因为date1和date2格式为 年-月-日， 所以这里先把date1和date2转换为时间戳再进行比较
                let date1 = new Date(this.query.beginDate).getTime();

                let date2 = new Date(this.query.endDate).getTime();
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
            }
        },
    };
</script>