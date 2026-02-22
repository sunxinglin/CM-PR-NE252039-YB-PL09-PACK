<template>
  <div>
    <div class="app-container">
      <el-col :span="24">
        <el-card shadow="never" class="boby-small" style="height: 100%">
          <div slot="header" class="clearfix">
            <span>自动拧紧任务详情</span>
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
            <el-table :data="tightenData" ref="dpTable" row-key="id" @current-change="handleSelectiondpChange" border fit
              stripe highlight-current-row align="left">
              <el-table-column property="paramName" align="center" label="任务名称"></el-table-column>
              <el-table-column property="programNo" align="center" label="程序号"></el-table-column>
              <el-table-column property="useNum" align="center" label="使用数量"></el-table-column>
              <el-table-column property="torqueMin" align="center" label="扭力下限"></el-table-column>
              <el-table-column property="torqueMax" align="center" label="扭力上限"></el-table-column>
              <el-table-column property="angleMin" align="center" label="角度下限"></el-table-column>
              <el-table-column property="angleMax" align="center" label="角度上限"></el-table-column>
              <el-table-column property="upMesCode" align="center" label="上传代码"></el-table-column>
            </el-table>
          </div>
        </el-card>
      </el-col>
    </div>

    <el-dialog v-el-drag-dialog class="dialog-mini" width="600px" :title="textMap[dialogStatus]"
      :visible.sync="dialogtightenVisible">
      <div>
        <el-form :rules="tightenRules" ref="dpForm" :model="tightenTemp" label-position="right" label-width="100px">
          <el-form-item size="small" :label="'任务名称'" prop="programNo">
            <el-input v-model="tightenTemp.paramName"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'程序号'" prop="programNo">
            <el-input v-model="tightenTemp.programNo"></el-input>
          </el-form-item>

          <el-form-item size="small" :label="'使用数量'" prop="useNum">
            <el-input v-model="tightenTemp.useNum"></el-input>
          </el-form-item>

          <el-form-item size="small" :label="'扭力下限'" >
            <el-input v-model="tightenTemp.torqueMin"></el-input>
          </el-form-item>

          <el-form-item size="small" :label="'扭力上限'" >
            <el-input v-model="tightenTemp.torqueMax"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'角度下限'" >
            <el-input v-model="tightenTemp.angleMin"></el-input>
          </el-form-item>

          <el-form-item size="small" :label="'角度上限'" >
            <el-input v-model="tightenTemp.angleMax"></el-input>
          </el-form-item>

          <el-form-item size="small" :label="'上传代码'" prop="upMesCode">
            <el-input v-model="tightenTemp.upMesCode"></el-input>
          </el-form-item>
        </el-form>
      </div>
      <div slot="footer">
        <el-button size="mini" @click="dialogtightenVisible = false">取消</el-button>
        <el-button size="mini" v-if="dialogStatus == 'create'" type="primary" @click="createDpData">确认</el-button>
        <el-button size="mini" v-else type="primary" @click="updateDpData">确认</el-button>
      </div>
    </el-dialog>
    <!-- 电批弹框2-->
  </div>
</template>

<script>
import * as stationTaskAutotighten from "@/api/stationTaskAuto.js";
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
      tightenTemp: {
        id: undefined,
        paramName:"",
        stationTaskId: this.taskId,
        useNum: 1,
        programNo: 1,
        torqueMin: 0,
        torqueMax: 999,
        angleMin: 0,
        angleMax: 9999,
        upMesCode: "",
      },
      textMap: {
        update: "编辑",
        create: "添加",
        detail: "任务详情",
      },

      tightenRules: {
        //编辑框输入限制
        programNo: [
          {
            required: true,
            message: "程序号不能为空",
            trigger: "blur",
          },
        ],
        useNum: [
          {
            required: true,
            message: "使用数量不能为空",
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
      tightenData: [],
      dialogtightenVisible: false,
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
      this.tightenTemp = {
        id: undefined,
        parameterName: "",
        tightenType: 1,
        stationTaskId: this.taskId,
        tightenLocate: 1,
        minValue: 0,
        maxValue: 999,
        upMesCode: "",
      };
    },

    handledpAdd() {
      this.dialogStatus = "create"; //编辑框功能选择（添加）
      this.dialogtightenVisible = true; //编辑框显示
      this.$nextTick(() => {
        this.$refs["dpForm"].clearValidate();
      });
      this.resetdpdata();
    },
    handledpEdit() {
      if (this.tightenTemp.id != undefined) {
        this.dialogStatus = "update"; //编辑框功能选择（添加）
        this.dialogtightenVisible = true; //编辑框显示
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
      if (this.tightenTemp.id === undefined) {
        this.$message({
          message: "请选择一个想要删除的数据",
          type: "error",
        });
        return;
      }
      this.$confirm("确定要删除吗？").then((_) => {
        //提取复选框的数据的Id
        var selectids = [];
        selectids.push(this.tightenTemp.id); //提取复选框的数据的Id
        var params = {
          ids: selectids,
        };
        stationTaskAutotighten.del(params).then(() => {
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
        this.tightenTemp = val;
      }
    },
    updateDpData() {
      this.$refs["dpForm"].validate((valid) => {
        if (valid) {
          stationTaskAutotighten.update(this.tightenTemp).then((response) => {
            this.dialogtightenVisible = false; //编辑框关闭
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
          stationTaskAutotighten.add(this.tightenTemp).then((response) => {
            this.dialogtightenVisible = false; //编辑框关闭
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
      stationTaskAutotighten.GetByTaskId({ taskid: this.taskId }).then((response) => {
        if(response.code != 200)
        {
          this.$notify({
              title: "Error",
              message: response.message,
              type: "error",
              duration: 2000,
            });
            return ;
        }
        this.tightenData = response.result; //提取数据表
      });
      this.$nextTick(() => { });
    },

    //#endregion
    back() {
      this.$parent.autoTightenVisiable = false;
      this.$parent.taskvisible = true;
      this.taskId = 0;
      this.stepId = 0;
    },
  },
};
</script>

<style></style>