<template>
  <div>
    <div class="app-container">
      <el-col :span="24">
        <el-card shadow="never" class="boby-small" style="height: 100%">
          <div slot="header" class="clearfix">
            <span>超时检测任务详情</span>
          </div>
          <div style="margin-bottom: 10px">
            <el-row :gutter="4">
              <el-button type="warning" icon="el-icon-back" size="mini" @click="back">返回</el-button>
              <el-button type="primary" icon="el-icon-plus" size="mini" @click="handleWeightAdd">添加</el-button>
              <el-button type="primary" icon="el-icon-edit" size="mini" @click="handleWeightEdit">编辑</el-button>
              <el-button type="primary" icon="el-icon-delete" size="mini" @click="handleWeightDelete">删除</el-button>
            </el-row>
          </div>
          <div>
            <el-table :data="outtimedata" ref="dpTable" row-key="id" @row-click="weightrowclick"
              @current-change="handleSelectionweightChange" border fit stripe highlight-current-row align="left">
              <el-table-column property="timeOutTaskName" label="详情名称" align="center"></el-table-column>
              <el-table-column property="timeOutFlag" label="超时标志" align="center"></el-table-column>
              <el-table-column property="minDuration" label="最小时长" align="center"></el-table-column>
              <el-table-column property="maxDuration" label="最大时长" align="center"></el-table-column>
              <el-table-column property="upMesCode" label="上传代码" align="center"></el-table-column>
            </el-table>
          </div>

        </el-card>
      </el-col>
    </div>
    <!-- </el-dialog> -->
    <!-- 超时列表结束 -->
    <!-- 超时表单开始 -->
    <el-dialog v-el-drag-dialog class="dialog-mini" width="600px" :title="textMap[dialogStatus]"
      :visible.sync="dialogTimeOutVisible">
      <div>
        <el-form :rules="timeoutRules" ref="gluingtimeForm" :model="timeout" label-position="right" label-width="100px">
          <el-form-item size="small" :label="'详情名称'" prop="timeOutTaskName">
            <el-input v-model="timeout.timeOutTaskName"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'超时标志'" prop="timeOutFlag">
            <el-input v-model="timeout.timeOutFlag"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'最小时长'">
            <el-input type="number" v-model="timeout.minDuration"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'最大时长'">
            <el-input type="number" v-model="timeout.maxDuration"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'上传代码'" prop="upMesCode">
            <el-input v-model="timeout.upMesCode"></el-input>
          </el-form-item>
        </el-form>
      </div>
      <div slot="footer">
        <el-button size="mini" @click="dialogTimeOutVisible = false">取消</el-button>
        <el-button size="mini" v-if="dialogStatus == 'create'" type="primary" @click="createTimeoutCheckData">确认</el-button>
        <el-button size="mini" v-else type="primary" @click="updateTimeoutCheckData">确认</el-button>
      </div>
    </el-dialog>
    <!-- 电子秤表单结束 -->
  </div>
</template>

<script>
import * as stationTaskMaxTime from "@/api/stationTaskMaxTime";
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
      dialogTimeOutVisible: false,
      weightTemp: {},
      timeout: {
        id: undefined,
        timeOutTaskName: "",
        maxDuration: 10,
        timeOutFlag: "",
        minDuration: 0,
        stationTaskId: this.taskId,
        upMesCode: ""
      },
      timeoutRules: {
        timeOutTaskName: [
          {
            required: true,
            message: "详情名称不能为空",
            trigger: "blur",
          }
        ],
        timeOutFlag: [{
          required: true,
          message: "超时标志不能为空",
          trigger: "blur",
        }],
        upMesCode: [
          {
            required: true,
            message: "上传代码不能为空",
            trigger: "blur",
          }
        ]
      },
      dialogStatus: "", //编辑框功能(添加/编辑)
      outtimedata: [],
      taskId: 0,
    };
  },
  mounted() {
    this.reloadTimeoutCheckData();
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
    //#region 电子秤

    resetouttimedata() {
      this.timeout = {
        id: undefined,
        timeOutTaskName: "",
        maxDuration: 10,
        timeOutFlag: "",
        minDuration: 0,
        stationTaskId: this.taskId,
        upMesCode: ""
      };
    },

    handleWeightAdd() {
      if (this.outtimedata.length >= 1) {
        this.$message({
          message: "只可添加一个任务",
          type: "error",
        });
        return;
      }
      this.dialogStatus = "create"; //编辑框功能选择（添加）
      this.dialogTimeOutVisible = true; //编辑框显示
      this.$nextTick(() => {
        this.$refs["gluingtimeForm"].clearValidate();
      });
      this.resetouttimedata();
    },
    handleWeightEdit() {
      if (this.weightTemp.id != undefined) {
        this.dialogStatus = "update"; //编辑框功能选择（添加）
        this.dialogTimeOutVisible = true; //编辑框显示
        this.$nextTick(() => {
          this.$refs["gluingtimeForm"].clearValidate();
        });
        this.resetouttimedata();
        this.timeout = this.weightTemp;
      } else {
        this.$message({
          message: "请选择一个想要修改的数据",
          type: "error",
        });
      }
    },
    handleWeightDelete() {
      if (this.weightTemp.id === undefined) {
        this.$message({
          message: "请选择一个想要删除的数据",
          type: "error",
        });
        return;
      }
      this.$confirm("确定要删除吗？")
        .then((_) => {
          //提取复选框的数据的Id
          var selectids = [];
          selectids.push(this.weightTemp.id); //提取复选框的数据的Id
          var params = {
            ids: selectids,
          };
          stationTaskMaxTime.del(params).then(() => {
            this.$notify({
              title: "成功",
              message: "删除成功",
              type: "success",
              duration: 2000,
            });
            this.reloadTimeoutCheckData();
            //页面加载
          });
        })
        .catch((_) => { });
    },
    handleSelectionweightChange(val) {
      if (val === null) {
        return;
      } else {
        this.weightTemp = val;
      }
    },
    weightrowclick(row) {
      this.weightTemp = row;
    },
    updateTimeoutCheckData() {
      this.$refs["gluingtimeForm"].validate((valid) => {
        if (valid) {
          stationTaskMaxTime.update(this.timeout).then((response) => {
            this.dialogTimeOutVisible = false; //编辑框关闭
            this.$notify({
              title: "成功",
              message: "修改成功",
              type: "success",
              duration: 2000,
            });
            this.resetouttimedata();
            this.reloadTimeoutCheckData();
          });
        }

      });
    },
    createTimeoutCheckData() {
      this.$refs["gluingtimeForm"].validate((valid) => {
        console.log(144);
        if (valid) {
          stationTaskMaxTime.add(this.timeout).then((response) => {
            this.dialogTimeOutVisible = false; //编辑框关闭
            this.$notify({
              title: "成功",
              message: "创建成功",
              type: "success",
              duration: 2000,
            });
            this.resetouttimedata();
            this.reloadTimeoutCheckData();
          });
        }
      });
    },
    reloadTimeoutCheckData() {
      if (this.taskId == 0) {
        this.taskId = this.$parent.taskId;
      }
      stationTaskMaxTime.load({ taskid: this.taskId }).then((response) => {
        this.outtimedata = response.data; //提取数据表
      });
      this.$nextTick(() => { });
    },
    back() {
      this.taskId = 0;
      this.$parent.timeoutvisiable = false;
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