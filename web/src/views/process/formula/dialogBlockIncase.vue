<template>
  <div>
    <div class="app-container">
      <el-col :span="24">
        <el-card shadow="never" class="boby-small" style="height: 100%">
          <div slot="header" class="clearfix">
            <span>模组入箱任务详情</span>
          </div>
          <div style="margin-bottom: 10px">
            <el-row :gutter="4">
              <el-col :span="20">
                <el-button type="warning" icon="el-icon-back" size="mini" @click="back">返回</el-button>
                <el-button type="primary" icon="el-icon-plus" size="mini" @click="handledpAdd">添加</el-button>
                <el-button type="primary" icon="el-icon-edit" size="mini" @click="handledpEdit">编辑</el-button>
                <el-button type="primary" icon="el-icon-delete" size="mini" @click="handledpDelete">删除</el-button>
              </el-col>
            </el-row>
          </div>
          <div>
            <el-table :data="blockInCaseData" ref="dpTable" row-key="id" @current-change="handleSelectiondpChange"
              border fit stripe highlight-current-row align="left">
              <el-table-column property="parameterName" align="center" label="任务名称"></el-table-column>
              <el-table-column property="location" align="center" label="模组位置"></el-table-column>
              <el-table-column prop="moduleInBoxDataType" align="center" label="数据类型"
                :formatter="setDatatype"></el-table-column>
              <el-table-column property="modulePN" align="center" label="模组PN"></el-table-column>
              <el-table-column property="minValue" align="center" label="最小值"></el-table-column>
              <el-table-column property="maxValue" align="center" label="最大值"></el-table-column>
              <el-table-column property="upMesCode" align="center" label="上传代码"></el-table-column>
            </el-table>
          </div>
        </el-card>
      </el-col>
    </div>

    <el-dialog v-el-drag-dialog class="dialog-mini" width="600px" :title="textMap[dialogStatus]"
      :visible.sync="dialogblockInCaseVisible">
      <div>
        <el-form :rules="blockInCaseRules" ref="dpForm" :model="blockInCaseTemp" label-position="right"
          label-width="100px">
          <el-form-item size="small" :label="'任务名称'" prop="programNo">
            <el-input v-model="blockInCaseTemp.parameterName"></el-input>
          </el-form-item>


          <el-form-item size="small" :label="'模组位置'">
            <el-input v-model="blockInCaseTemp.location"></el-input>
          </el-form-item>

          <el-form-item size="small" :label="'数据类型'" prop="glueType">
            <el-select v-model="blockInCaseTemp.moduleInBoxDataType" placeholder="请选择">
              <el-option v-for="item in typeoptions" :key="item.id" :label="item.name" :value="item.id">
              </el-option>
            </el-select>
          </el-form-item>

          <el-form-item size="small" :label="'模组PN'">
            <el-input v-model="blockInCaseTemp.modulePN"></el-input>
          </el-form-item>

          <el-form-item size="small" :label="'最小值'">
            <el-input v-model="blockInCaseTemp.minValue"></el-input>
          </el-form-item>

          <el-form-item size="small" :label="'最大值'">
            <el-input v-model="blockInCaseTemp.maxValue"></el-input>
          </el-form-item>

          <el-form-item size="small" :label="'上传代码'" prop="upMesCode">
            <el-input v-model="blockInCaseTemp.upMesCode"></el-input>
          </el-form-item>
        </el-form>
      </div>
      <div slot="footer">
        <el-button size="mini" @click="dialogblockInCaseVisible = false">取消</el-button>
        <el-button size="mini" v-if="dialogStatus == 'create'" type="primary" @click="createDpData">确认</el-button>
        <el-button size="mini" v-else type="primary" @click="updateDpData">确认</el-button>
      </div>
    </el-dialog>
    <!-- 电批弹框2-->
  </div>
</template>

<script>
import * as stationTaskAutoblockInCase from "@/api/stationTaskBlockInCase.js";
import * as proresource from "@/api/proresource.js";

import elDragDialog from "@/directive/el-dragDialog";
import waves from "@/directive/waves"; // 水波纹指令
export default {
  directives: {
    waves,
    elDragDialog,
  },
  data() {

    return {
      blockInCaseTemp: {
        id: undefined,
        stationTaskId: this.taskId,
        parameterName: "",
        minValue: 0,
        maxValue: 999,
        upMesCode: "",
        location: 1,
        moduleInBoxDataType: 1,
        modulePN: '',
        moduleType: 1,
      },
      textMap: {
        update: "编辑",
        create: "添加",
        detail: "任务详情",
      },

      typeoptions: [
        {
          id: 1,
          name: "模组码",
        },
        {
          id: 2,
          name: "模组长度",
        },
        {
          id: 3,
          name: "保压时间",
        },
        {
          id: 4,
          name: "下压距离",
        },
        {
          id: 5,
          name: "下压压力",
        },
        {
          id: 6,
          name: "左侧压力",
        },
        {
          id: 7,
          name: "右侧压力",
        },
        {
          id: 8,
          name: "入完成时间",
        },
        {
          id: 9,
          name: "模组入箱时长",
        },
      ],
      blockInCaseRules: {
        upMesCode: [
          {
            required: true,
            message: "上传代码不能为空",
            trigger: "blur",

          },
        ],
      },
      dialogStatus: "", //编辑框功能(添加/编辑)
      blockInCaseData: [],
      dialogblockInCaseVisible: false,
      taskId: 0,
      stepId: 0,
    };
  },
  mounted() {
    this.Load();

  },
  methods: {
    //#region 拧紧枪

    resetdpdata() {
      this.$refs.dpTable.setCurrentRow();
      this.blockInCaseTemp = {
        id: undefined,
        parameterName: "",
        blockInCaseType: 1,
        stationTaskId: this.taskId,
        blockInCaseLocate: 1,
        minValue: 0,
        maxValue: 999,
        upMesCode: "",
        location: 1,
        moduleInBoxDataType: 1,
        modulePN: '',
        moduleType: 1,
      };
    },
    setDatatype(row, column, cellValue) {
      switch (cellValue) {
        case 1:
          return "模组码";
        case 2:
          return "模组长度";
        case 3:
          return "保压时间";
        case 4:
          return "下压距离";
        case 5:
          return "下压压力";
        case 6:
          return "左侧压力";
        case 7:
          return "右侧压力";
        case 8:
          return "入箱完成时间";
        case 9:
          return "模组入箱时长";
        default:
          return null;
      }
    },
    handledpAdd() {
      this.dialogStatus = "create"; //编辑框功能选择（添加）
      this.dialogblockInCaseVisible = true; //编辑框显示
      this.$nextTick(() => {
        this.$refs["dpForm"].clearValidate();
      });
      this.resetdpdata();
    },
    handledpEdit() {
      if (this.blockInCaseTemp.id != undefined) {
        this.dialogStatus = "update"; //编辑框功能选择（添加）
        this.dialogblockInCaseVisible = true; //编辑框显示
        this.$nextTick(() => {
          this.$refs["dpForm"].clearValidate();
        });
      } else {
        this.$message({
          message: "请选择一个想要修改的数据",
          type: "error",
        });
      }
    },
    handledpDelete() {
      if (this.blockInCaseTemp.id === undefined) {
        this.$message({
          message: "请选择一个想要删除的数据",
          type: "error",
        });
        return;
      }
      this.$confirm("确定要删除吗？").then((_) => {
        //提取复选框的数据的Id
        var selectids = [];
        selectids.push(this.blockInCaseTemp.id); //提取复选框的数据的Id
        var params = {
          ids: selectids,
        };
        stationTaskAutoblockInCase.del(params).then(() => {
          this.$notify({
            title: "成功",
            message: "删除成功",
            type: "success",
            duration: 2000,
          });
          this.Load();
          //页面加载
        });
      });
    },
    handleSelectiondpChange(val) {
      if (val === null) {
        return;
      } else {
        this.blockInCaseTemp = val;
      }
    },
    updateDpData() {
      this.$refs["dpForm"].validate((valid) => {
        if (valid) {
          stationTaskAutoblockInCase.update(this.blockInCaseTemp).then((response) => {
            this.dialogblockInCaseVisible = false; //编辑框关闭
            this.$notify({
              title: "成功",
              message: "修改成功",
              type: "success",
              duration: 2000,
            });
            this.Load();
            this.resetdpdata();
          });
        }
      });
    },
    createDpData() {
      this.$refs["dpForm"].validate((valid) => {
        if (valid) {
          stationTaskAutoblockInCase.add(this.blockInCaseTemp).then((response) => {
            this.dialogblockInCaseVisible = false; //编辑框关闭
            this.$notify({
              title: "成功",
              message: "创建成功",
              type: "success",
              duration: 2000,
            });
            this.Load();
            this.resetdpdata();
          });
        }
      });
    },
    Load() {
      if (this.taskId == 0) {
        this.stepId = this.$parent.stepId;
        this.taskId = this.$parent.taskId;
      }
      stationTaskAutoblockInCase.GetByTaskId({ taskid: this.taskId }).then((response) => {
        if (response.code != 200) {
          this.$notify({
            title: "Error",
            message: response.message,
            type: "error",
            duration: 2000,
          });
          return;
        }
        this.blockInCaseData = response.result; //提取数据表
      });
      this.$nextTick(() => { });
    },

    //#endregion
    back() {
      this.$parent.blockIncaseVisiable = false;
      this.$parent.taskvisible = true;
      this.taskId = 0;
      this.stepId = 0;
    },
  },
};
</script>

<style></style>