<template>
    <el-row class="panel-group" :gutter="40">
        <el-col :xs="12" :sm="12" :lg="6" class="card-panel-col">
            <div @click="btnClick('/productionrecord/production')">
                <div class='card-panel' @click="handleSetLineChartData('newVisitis')">
                    <div class="card-panel-icon-wrapper icon-people">
                        <svg-icon icon-class="peoples" class-name="card-panel-icon" />
                    </div>
                    <div class="card-panel-description">
                        <div class="card-panel-text">今日Pack数量</div>
                        <count-to class="card-panel-num" :startVal="0" :endVal="list.todayOrder" :duration="2400"></count-to>
                    </div>
                </div>
            </div>
        </el-col>
        <el-col :xs="12" :sm="12" :lg="6" class="card-panel-col">
            <div class="card-panel" @click="handleSetLineChartData('messages')">
                <div class="card-panel-icon-wrapper icon-message">
                    <svg-icon icon-class="message" class-name="card-panel-icon" />
                </div>
                <div class="card-panel-description">
                    <div class="card-panel-text">今日NG下线</div>
                    <count-to class="card-panel-num" :startVal="0" :endVal="list.todayOrderNG" :duration="3000"></count-to>
                </div>
            </div>
        </el-col>
        <el-col :xs="12" :sm="12" :lg="6" class="card-panel-col">
            <div class="card-panel" @click="handleSetLineChartData('purchases')">
                <div class="card-panel-icon-wrapper icon-money">
                    <svg-icon icon-class="money" class-name="card-panel-icon" />
                </div>
                <div class="card-panel-description">
                    <div class="card-panel-text">今日下线数量</div>
                    <count-to class="card-panel-num" :startVal="0" :endVal="list.todayWeight" :duration="3200"></count-to>
                </div>
            </div>
        </el-col>
        <el-col :xs="12" :sm="12" :lg="6" class="card-panel-col">
            <div class="card-panel" @click="handleSetLineChartData('shoppings')">
                <div class="card-panel-icon-wrapper icon-shoppingCard">
                    <svg-icon icon-class="shoppingCard" class-name="card-panel-icon" />
                </div>
                <div class="card-panel-description">
                    <div class="card-panel-text">今日下线NG</div>
                    <count-to class="card-panel-num" :startVal="0" :endVal="list.todayWeightNG" :duration="3600"></count-to>
                </div>
            </div>
        </el-col>



        <el-col :xs="12" :sm="12" :lg="6" class="card-panel-col">
            <div @click="btnClick('/productionrecord/production')">
                <div class='card-panel' @click="handleSetLineChartData('newVisitis')">
                    <div class="card-panel-icon-wrapper icon-people">
                        <svg-icon icon-class="peoples" class-name="card-panel-icon" />
                    </div>
                    <div class="card-panel-description">
                        <div class="card-panel-text">本周Pack数量</div>
                        <count-to class="card-panel-num" :startVal="0" :endVal="list.weekOrder" :duration="3600"></count-to>
                    </div>
                </div>
            </div>
        </el-col>
        <el-col :xs="12" :sm="12" :lg="6" class="card-panel-col">
            <div class="card-panel" @click="handleSetLineChartData('messages')">
                <div class="card-panel-icon-wrapper icon-message">
                    <svg-icon icon-class="message" class-name="card-panel-icon" />
                </div>
                <div class="card-panel-description">
                    <div class="card-panel-text">本周NG下线</div>
                    <count-to class="card-panel-num" :startVal="0" :endVal="list.weekOrderNG" :duration="3600"></count-to>
                </div>
            </div>
        </el-col>
        <el-col :xs="12" :sm="12" :lg="6" class="card-panel-col">
            <div class="card-panel" @click="handleSetLineChartData('purchases')">
                <div class="card-panel-icon-wrapper icon-money">
                    <svg-icon icon-class="money" class-name="card-panel-icon" />
                </div>
                <div class="card-panel-description">
                    <div class="card-panel-text">本周下线数量</div>
                    <count-to class="card-panel-num" :startVal="0" :endVal="list.weekWeight" :duration="3600"></count-to>
                </div>
            </div>
        </el-col>
        <el-col :xs="12" :sm="12" :lg="6" class="card-panel-col">
            <div class="card-panel" @click="handleSetLineChartData('shoppings')">
                <div class="card-panel-icon-wrapper icon-shoppingCard">
                    <svg-icon icon-class="shoppingCard" class-name="card-panel-icon" />
                </div>
                <div class="card-panel-description">
                    <div class="card-panel-text">本周下线NG</div>
                    <count-to class="card-panel-num" :startVal="0" :endVal="list.weekWeightNG" :duration="3600"></count-to>
                </div>
            </div>
        </el-col>


        <el-col :xs="12" :sm="12" :lg="6" class="card-panel-col">
            <div @click="btnClick('/productionrecord/production')">
                <div class='card-panel' @click="handleSetLineChartData('newVisitis')">
                    <div class="card-panel-icon-wrapper icon-people">
                        <svg-icon icon-class="peoples" class-name="card-panel-icon" />
                    </div>
                    <div class="card-panel-description">
                        <div class="card-panel-text">本月Pack数量</div>
                        <count-to class="card-panel-num" :startVal="0" :endVal="list.monthOrder" :duration="3600"></count-to>
                    </div>
                </div>
            </div>
        </el-col>
        <el-col :xs="12" :sm="12" :lg="6" class="card-panel-col">
            <div class="card-panel" @click="handleSetLineChartData('messages')">
                <div class="card-panel-icon-wrapper icon-message">
                    <svg-icon icon-class="message" class-name="card-panel-icon" />
                </div>
                <div class="card-panel-description">
                    <div class="card-panel-text">本月NG下线</div>
                    <count-to class="card-panel-num" :startVal="0" :endVal="list.monthOrderNG" :duration="3600"></count-to>
                </div>
            </div>
        </el-col>
        <el-col :xs="12" :sm="12" :lg="6" class="card-panel-col">
            <div class="card-panel" @click="handleSetLineChartData('purchases')">
                <div class="card-panel-icon-wrapper icon-money">
                    <svg-icon icon-class="money" class-name="card-panel-icon" />
                </div>
                <div class="card-panel-description">
                    <div class="card-panel-text">本月下线数量</div>
                    <count-to class="card-panel-num" :startVal="0" :endVal="list.monthWeight" :duration="3600"></count-to>
                </div>
            </div>
        </el-col>
        <el-col :xs="12" :sm="12" :lg="6" class="card-panel-col">
            <div class="card-panel" @click="handleSetLineChartData('shoppings')">
                <div class="card-panel-icon-wrapper icon-shoppingCard">
                    <svg-icon icon-class="shoppingCard" class-name="card-panel-icon" />
                </div>
                <div class="card-panel-description">
                    <div class="card-panel-text">本月下线NG</div>
                    <count-to class="card-panel-num" :startVal="0" :endVal="list.monthWeightNG" :duration="3600"></count-to>
                </div>
            </div>
        </el-col>

    </el-row>




</template>

<script>
    import CountTo from 'vue-count-to'
    import * as alarmlog from "@/api/alarmlog";

    export default {

        components: {
            CountTo
        },
        data() {
            return {
                listLoading: true, //加载特效
                tableKey: 0,
                list: "",
                total: 0,
            };
        },
        mounted() {
            this.getList();
        },
        methods: {
            btnClick(rount) {
                const vm = this;
                vm.$router.push({
                    path: rount,
                });
            },
            handleFilter() {
                this.getlist();
            },
            handleSetLineChartData(type) {
                this.$emit('handleSetLineChartData', type)
            },
            getList() {
                alarmlog.GetOutPut(this.listQuery).then((response) => {
                    console.log(response.data);
                    console.log(response)
                    this.list = response.data;
                });
            },

        }
    }
</script>

<style rel="stylesheet/scss" lang="scss" scoped>
    .panel-group {
        margin-top: 18px;

        .card-panel-col {
            margin-bottom: 32px;
        }

        .card-panel {
            height: 108px;
            cursor: pointer;
            font-size: 12px;
            position: relative;
            overflow: hidden;
            color: #666;
            background: #fff;
            box-shadow: 4px 4px 40px rgba(0, 0, 0, .05);
            border-color: rgba(0, 0, 0, .05);

            &:hover {
                .card-panel-icon-wrapper {
                    color: #fff;
                }

                .icon-people {
                    background: #40c9c6;
                }

                .icon-message {
                    background: #36a3f7;
                }

                .icon-money {
                    background: #f4516c;
                }

                .icon-shoppingCard {
                    background: #34bfa3
                }
            }

            .icon-people {
                color: #40c9c6;
            }

            .icon-message {
                color: #36a3f7;
            }

            .icon-money {
                color: #f4516c;
            }

            .icon-shoppingCard {
                color: #34bfa3
            }

            .card-panel-icon-wrapper {
                float: left;
                margin: 14px 0 0 14px;
                padding: 16px;
                transition: all 0.38s ease-out;
                border-radius: 6px;
            }

            .card-panel-icon {
                float: left;
                font-size: 48px;
            }

            .card-panel-description {
                float: right;
                font-weight: bold;
                margin: 26px;
                margin-left: 0px;

                .card-panel-text {
                    line-height: 18px;
                    color: rgba(0, 0, 0, 0.45);
                    font-size: 16px;
                    margin-bottom: 12px;
                }

                .card-panel-num {
                    font-size: 20px;
                }
            }
        }
    }
</style>
