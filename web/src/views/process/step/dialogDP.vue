<template>
  <div>
    <div class="app-container">
      <el-col :span="24">
        <el-card shadow="never" class="boby-small" style="height: 100%">
          <div slot="header" class="clearfix">
            <span>拧螺丝任务详情</span>
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
            <el-table :data="dpData" ref="dpTable" row-key="id" @current-change="handleSelectiondpChange" border fit
              stripe highlight-current-row align="left">
              <el-table-column property="screwSpecs" label="名称" align="center" width="160"></el-table-column>
              <el-table-column property="programNo" align="center" label="程序号"></el-table-column>
              <el-table-column property="deviceNos" align="center" label="枪号"></el-table-column>
              <el-table-column property="taoTongNo" label="套筒号" align="center"></el-table-column>
              <el-table-column property="reworkLimitTimes" label="返工次数" align="center"></el-table-column>
              <el-table-column property="torqueMinLimit" label="最小扭矩" align="center"></el-table-column>
              <el-table-column property="torqueMaxLimit" label="最大扭矩" align="center"></el-table-column>
              <el-table-column property="angleMinLimit" label="最小角度" align="center"></el-table-column>
              <el-table-column property="angleMaxLimit" label="最大角度" align="center"></el-table-column>
              <el-table-column property="upMesCodePN" label="上传代码" align="center"></el-table-column>
              <el-table-column property="useNum" label="使用次数" align="center"></el-table-column>
              <el-table-column property="upMESCodeStartNo" label="螺丝开始序号" align="center"></el-table-column>

            </el-table>
          </div>
        </el-card>
      </el-col>
    </div>
    <!-- </el-dialog> -->
    <!-- 电批弹框1-->
    <!-- 电批弹框2-->
    <el-dialog v-el-drag-dialog class="dialog-mini" width="600px" :title="textMap[dialogStatus]"
      :visible.sync="dialogDpVisible">
      <div>
        <el-form :rules="anyloadRules" ref="dpForm" :model="dpTemp" label-position="right" label-width="100px">
          <el-form-item size="small" :label="'螺丝规格'" prop="screwSpecs">
            <el-input v-model="dpTemp.screwSpecs"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'使用次数'" prop="useNum">
            <el-input v-model="dpTemp.useNum"></el-input>
          </el-form-item>

          <el-form-item size="small" :label="'枪号'" prop="deviceNos">
            <el-input v-model="dpTemp.deviceNos"></el-input>
          </el-form-item>

          <el-form-item size="small" :label="'程序号'" prop="programNo">
            <el-input v-model="dpTemp.programNo"></el-input>
          </el-form-item>

          <el-form-item size="small" :label="'套筒号'" prop="taoTongNo">
            <el-input v-model="dpTemp.taoTongNo"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'返工次数'" prop="reworkLimitTimes">
            <el-input v-model="dpTemp.useNum"></el-input>
          </el-form-item>

          <el-form-item size="small" :label="'最小扭矩'" prop="torqueMinLimit">
            <el-input v-model="dpTemp.torqueMinLimit"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'最大扭矩'" prop="torqueMaxLimit">
            <el-input v-model="dpTemp.torqueMaxLimit"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'最小角度'" prop="angleMinLimit">
            <el-input v-model="dpTemp.angleMinLimit"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'最大角度'" prop="angleMaxLimit">
            <el-input v-model="dpTemp.angleMaxLimit"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'上传代码'" prop="upMesCodePN">
            <el-input v-model="dpTemp.upMesCodePN"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'螺丝开始序号'" prop="upMESCodeStartNo">
            <el-input v-model="dpTemp.upMESCodeStartNo"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'备注'">
            <el-input v-model="dpTemp.remark"></el-input>
          </el-form-item>
        </el-form>
      </div>
      <div slot="footer">
        <el-button size="mini" @click="dialogDpVisible = false">取消</el-button>
        <el-button size="mini" v-if="dialogStatus == 'create'" type="primary" @click="createDpData">确认</el-button>
        <el-button size="mini" v-else type="primary" @click="updateDpData">确认</el-button>
      </div>
    </el-dialog>
    <!-- 电批弹框2-->
  </div>
</template>

<script>
import * as stationtaskscrew from "@/api/stationtaskscrew.js";
import * as proresource from "@/api/proresource.js";

import elDragDialog from "@/directive/el-dragDialog";
import waves from "@/directive/waves"; // 水波纹指令
export default {
  directives: {
    waves,
    elDragDialog,
  },
  data() {
    var checkPositivenumber = (rule, value, callback) => {
      console.log(value);
      if (rule.field == "useNum") {
        if (Number(value) < 1) {
          callback(new Error("最小为1"));
        }
      }
      if (Number(value) < 0) {
        callback(new Error("最小为0"));
      }


      callback();
    };
    return {
      dpTemp: {
        id: undefined,
        screwSpecs: "",
        programNo: "",
        useNum: 1,
        remark: "",
        name: "",
        deviceNos:"",
        taoTongNo: "",
        stationTaskId: 0,
        reworkLimitTimes: 0,
        torqueMinLimit: "",
        torqueMaxLimit: "",
        angleMinLimit: "",
        angleMaxLimit: "",
        upMesCodePN: "",
        upMESCodeStartNo: 0,
      },
      textMap: {
        update: "编辑",
        create: "添加",
        detail: "任务详情",
      },
      stationBoltGunList: [], //拧紧枪列表
      stationBoltGunGroupList: [], //拧紧枪列表

      anyloadRules: {
        //编辑框输入限制
        useNum: [
          {
            required: true,
            message: "使用数量不能为空",
            trigger: "blur",
            validator: checkPositivenumber,
          },
        ],
        deviceNos: [
          {
            required: true,
            message: "枪号不能为空",
            trigger: "blur",

          },
        ],
        screwSpecs: [
          {
            required: true,
            message: "螺丝规格不能为空",
            trigger: "blur",

          },
        ],
        programNo: [
          {
            required: true,
            message: "程序号不能为空",
            trigger: "blur",

          },
        ],

        reworkLimitTimes: [
          {
            required: true,
            message: "返工次数不能为空",
            trigger: "blur",
            validator: checkPositivenumber,
          },
        ],
        torqueMinLimit: [
          {

            message: "最小扭矩不能为空",
            trigger: "blur",
            validator: checkPositivenumber,
          },
        ],
        torqueMaxLimit: [
          {

            message: "最大扭矩不能为空",
            trigger: "blur",
            validator: checkPositivenumber,
          },
        ],
        angleMinLimit: [
          {

            message: "最小角度不能为空",
            trigger: "blur",
            validator: checkPositivenumber,
          },
        ],
        angleMaxLimit: [
          {

            message: "最大角度不能为空",
            trigger: "blur",
            validator: checkPositivenumber,
          },
        ],
        upMesCodePN: [
          {
            required: true,
            message: "上传代码不能为空",
            trigger: "blur",
          },
        ],
        upMESCodeStartNo: [
          {
            required: true,
            message: "螺丝开始序号不能为空",
            trigger: "blur",
          },
        ],
      },

      dialogStatus: "", //编辑框功能(添加/编辑)
      dpData: [],
      resourceData: [],
      dialogDpVisible: false,
      taskId: 0,
      stepId: 0,
    };
  },
  mounted() {
    this.Load();
    this.loadresource();
  },
  methods: {
    //#region 拧紧枪

    resetdpdata() {
      this.dpTemp = {
        id: undefined,
        screwSpecs: "",
        programNo: "",
        useNum: 1,
        remark: "",
        name: "",
        deviceNos:"",
        taoTongNo: "",
        stationTaskId: this.taskId,
        reworkLimitTimes: 1,
        torqueMinLimit: "",
        torqueMaxLimit: "",
        angleMinLimit: "",
        angleMaxLimit: "",
        upMesCodePN: "",
        upMESCodeStartNo: 0,
      };
    },

    handledpAdd() {
      this.dialogStatus = "create"; //编辑框功能选择（添加）
      this.dialogDpVisible = true; //编辑框显示
      this.$nextTick(() => {
        this.$refs["dpForm"].clearValidate();
      });
      this.resetdpdata();
    },
    handledpEdit() {
      if (this.dpTemp.id != undefined) {
        this.dialogStatus = "update"; //编辑框功能选择（添加）
        this.dialogDpVisible = true; //编辑框显示
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
      if (this.dpTemp.id === undefined) {
        this.$message({
          message: "请选择一个想要删除的数据",
          type: "error",
        });
        return;
      }
      this.$confirm("确定要删除吗？").then((_) => {
        //提取复选框的数据的Id
        var selectids = [];
        selectids.push(this.dpTemp.id); //提取复选框的数据的Id
        var params = {
          ids: selectids,
        };
        stationtaskscrew.del(params).then(() => {
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
        this.dpTemp = val;
      }
    },

    updateDpData() {
      this.$refs["dpForm"].validate((valid) => {
        if (valid) {
          this.dpTemp.reworkLimitTimes = this.dpTemp.useNum;
          console.log(this.dpTemp)
          stationtaskscrew.update(this.dpTemp).then((response) => {
            this.dialogDpVisible = false; //编辑框关闭
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
          stationtaskscrew.add(this.dpTemp).then((response) => {
            this.dialogDpVisible = false; //编辑框关闭
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
      stationtaskscrew.Load({ taskid: this.taskId }).then((response) => {
        this.dpData = response.data; //提取数据表
        console.log(this.dpData);
      });
      this.$nextTick(() => { });
    },
    loadresource() {
      proresource
        .getlistbystepId({ stepId: this.stepId })
        .then((response) => {
          //   response.result.forEach((element) => {
          //     if (element.proResourceType == 2) {

          this.resourceData = response.result;
          //     }
          //   });
          console.log(this.resourceData);
        });
    },
    //#endregion
    back() {
      this.$parent.Dpvisibled = false;
      this.$parent.taskvisible = true;
      this.taskId = 0;
      this.stepId = 0;
    },
  },
};
</script>
 
<style></style>