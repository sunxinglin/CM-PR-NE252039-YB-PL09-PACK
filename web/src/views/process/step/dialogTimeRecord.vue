<template>
    <div>
        <div class="app-container">
            <el-col :span="24">
                <el-card shadow="never" class="boby-small" style="height: 100%">
                    <div slot="header" class="clearfix">
                        <span>时间记录任务详情</span>
                    </div>
                    <div style="margin-bottom: 10px">
                        <el-row :gutter="4">
                            <el-button type="warning" icon="el-icon-back" size="mini" @click="back">返回</el-button>
                            <el-button type="primary" icon="el-icon-plus" size="mini"
                                @click="handleRecordTimeAdd">添加</el-button>
                            <el-button type="primary" icon="el-icon-edit" size="mini"
                                @click="handleRecordTimeEdit">编辑</el-button>
                            <el-button type="primary" icon="el-icon-delete" size="mini"
                                @click="handleRecordTimeDelete">删除</el-button>
                        </el-row>
                    </div>
                    <div>
                        <el-table :data="RecordTimedata" ref="dpTable" row-key="id" @row-click="RecordTimerowclick"
                            @current-change="handleSelectionRecordTimeChange" border fit stripe highlight-current-row
                            align="left">
                            <el-table-column property="recordTimeTaskName" label="详情名称" align="center"></el-table-column>
                            <el-table-column property="timeOutFlag" label="时间标志" align="center"></el-table-column>
                            <el-table-column property="upMesCode" label="上传代码" align="center"></el-table-column>
                        </el-table>
                    </div>
                </el-card>
            </el-col>
        </div>

        <el-dialog v-el-drag-dialog class="dialog-mini" width="600px" :title="textMap[dialogStatus]"
            :visible.sync="dialogRecordTimeVisible">
            <div>
                <el-form :rules="RecordTimeRules" ref="RecordTimeForm" :model="recordTimeDto" label-position="right"
                    label-width="100px">
                    <el-form-item size="small" :label="'详情名称'" prop="recordTimeTaskName">
                        <el-input v-model="recordTimeDto.recordTimeTaskName"></el-input>
                    </el-form-item>
                    <el-form-item size="small" :label="'时间标志'" prop="timeOutFlag">
                        <el-input v-model="recordTimeDto.timeOutFlag"></el-input>
                    </el-form-item>

                    <el-form-item size="small" :label="'上传代码'" prop="upMesCode">
                        <el-input v-model="recordTimeDto.upMesCode"></el-input>
                    </el-form-item>
                </el-form>
            </div>
            <div slot="footer">
                <el-button size="mini" @click="dialogRecordTimeVisible = false">取消</el-button>
                <el-button size="mini" v-if="dialogStatus == 'create'" type="primary"
                    @click="createRecordTimeData">确认</el-button>
                <el-button size="mini" v-else type="primary" @click="updateRecordTimeData">确认</el-button>
            </div>
        </el-dialog>
    </div>
</template>
  
<script>
import * as taskTimeRecord from "@/api/stationTaskTimeRecord";
import elDragDialog from "@/directive/el-dragDialog";
import waves from "@/directive/waves"; // 水波纹指令
export default {
    directives: {
        waves,
        elDragDialog,
    },
    data() {
        return {
            textMap: {
                update: "编辑",
                create: "添加",
                detail: "任务详情",
            },
            dialogRecordTimeVisible: false,
            RecordTimeTemp: {},
            recordTimeDto: {
                id: undefined,
                recordTimeTaskName: "",
                stationTaskId: 0,
                upMesCode: '',
                timeOutFlag: "",
            },
            RecordTimeRules: {
                recordTimeTaskName: [
                    {
                        required: true,
                        message: "名称不能为空",
                        trigger: "blur",
                    },
                ],
                timeOutFlag: [
                    {
                        required: true,
                        message: "时间标志不能为空",
                        trigger: "blur",
                    },
                ],
                upMesCode: [
                    {
                        required: true,
                        message: "上传代码不能为空",
                        trigger: "blur",
                    },
                ],
            },
            dialogStatus: "", //编辑框功能(添加/编辑)
            RecordTimedata: [],
            taskId: 0,
        };
    },
    mounted() {
        this.reloadRecordTimeData();
    },
    methods: {
        //Bool转换
        formatterBoolean(row, column, cellValue) {
            if (cellValue) {
                return "是";
            } else {
                return "否";
            }
        },
        resetRecordTimedata() {
            this.recordTimeDto = {
                id: undefined,
                recordTimeTaskName: "",
                stationTaskId: 0,
                upMesCode: '',
                timeOutFlag: ""
            };
        },
        handleRecordTimeAdd() {
            if (this.RecordTimedata?.length >= 1) {
                this.$message({
                    message: "只可添加一个任务",
                    type: "error",
                });
                return
            }
            this.dialogStatus = "create"; //编辑框功能选择（添加）
            this.dialogRecordTimeVisible = true; //编辑框显示
            this.$nextTick(() => {
                this.$refs["RecordTimeForm"].clearValidate();
            });
            this.resetRecordTimedata();
        },
        handleRecordTimeEdit() {
            if (this.RecordTimeTemp.id != undefined) {
                this.dialogStatus = "update"; //编辑框功能选择（添加）
                this.dialogRecordTimeVisible = true; //编辑框显示
                this.$nextTick(() => {
                    this.$refs["RecordTimeForm"].clearValidate();
                });
                this.resetRecordTimedata();
                this.recordTimeDto = this.RecordTimeTemp;
            } else {
                this.$message({
                    message: "请选择一个想要修改的数据",
                    type: "error",
                });
            }
        },
        handleRecordTimeDelete() {
            if (this.RecordTimeTemp.id === undefined) {
                this.$message({
                    message: "请选择一个想要删除的数据",
                    type: "error",
                });
                return;
            }
            this.$confirm("确定要删除吗？")
                .then((_) => {
                    //提取复选框的数据的Id
                    taskTimeRecord.del(this.RecordTimeTemp).then(() => {
                        this.$notify({
                            title: "成功",
                            message: "删除成功",
                            type: "success",
                            duration: 2000,
                        });
                        this.reloadRecordTimeData();
                        //页面加载
                    });
                })
                .catch((_) => { });
        },
        handleSelectionRecordTimeChange(val) {
            if (val === null) {
                return;
            } else {
                this.RecordTimeTemp = val;
            }
        },
        RecordTimerowclick(row) {
            this.RecordTimeTemp = row;
        },
        updateRecordTimeData() {
            this.$refs["RecordTimeForm"].validate((valid) => {
                if (valid) {
                    taskTimeRecord.update(this.recordTimeDto).then((response) => {
                        this.dialogRecordTimeVisible = false; //编辑框关闭
                        this.$notify({
                            title: "成功",
                            message: "修改成功",
                            type: "success",
                            duration: 2000,
                        });
                        this.resetRecordTimedata();
                        this.reloadRecordTimeData();
                    });
                }

            });
        },
        createRecordTimeData() {
            this.$refs["RecordTimeForm"].validate((valid) => {
                if (valid) {
                     this.recordTimeDto.stationTaskId = this.taskId;
                    taskTimeRecord.add(this.recordTimeDto).then((response) => {
                        this.dialogRecordTimeVisible = false; //编辑框关闭
                        this.$notify({
                            title: "成功",
                            message: "创建成功",
                            type: "success",
                            duration: 2000,
                        });
                        this.resetRecordTimedata();
                        this.reloadRecordTimeData();
                    });
                }
            });
        },
        reloadRecordTimeData() {
            if (this.taskId == 0) {
                this.taskId = this.$parent.taskId;
            }
            taskTimeRecord.load({ stationTaskId: this.taskId }).then((response) => {
                this.RecordTimedata = response.result; //提取数据表
            });
            this.$nextTick(() => { });
        },

        //#endregion
        back() {
            this.taskId = 0;
            this.$parent.recordTimeVisible = false;
            this.$parent.taskvisible = true;
        },
    },
    props: {
        taskid: {
            type: String,
            default: "",
        },
    },
};
</script>
  
<style></style>