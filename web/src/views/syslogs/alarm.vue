<template>
    <div>
        <div class="app-container">
            <el-card shadow="never" class="boby-small" style="height: 100%">
                <div slot="header" class="clearfix">
                    <span>系统报警</span>
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

                <el-table-column label="标题" prop="name" align="center" min-width="30px" />
                <el-table-column label="发生时间" prop="occurTime" align="center" min-width="30px" />
                <el-table-column label="描述" prop="description" align="center" />

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
    import * as alarmLogAPI from "@/api/alarmlog";

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
                    beginDate: new Date(),
                    endDate: new Date()
                },
                tablegeight: null,
                total: 0,
                ListLoading: false,
                pickerOptions1: {},
                pickerOptions0: {},
            };
        },
        methods: {
            getList() {
                this.ListLoading = true;
                alarmLogAPI.GetPageList(this.query).then((response) => {
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