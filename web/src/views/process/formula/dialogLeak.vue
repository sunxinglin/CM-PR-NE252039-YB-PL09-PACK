<template>
  <div>
    <div class="app-container">
      <el-col :span="24">
        <el-card shadow="never" class="boby-small" style="height: 100%">
          <div slot="header" class="clearfix">
            <span>充气任务详情</span>
          </div>
          <div style="margin-bottom: 10px">
            <el-row :gutter="4">
              <el-col :span="20">
                <el-button
                  type="warning"
                  icon="el-icon-back"
                  size="mini"
                  @click="back"
                  >返回</el-button
                >
                <el-button
                  type="primary"
                  icon="el-icon-plus"
                  size="mini"
                  @click="handledpAdd"
                  >添加</el-button
                >
                <el-button
                  type="primary"
                  icon="el-icon-edit"
                  size="mini"
                  @click="handledpEdit"
                  >编辑</el-button
                >
                <el-button
                  type="primary"
                  icon="el-icon-delete"
                  size="mini"
                  @click="handledpDelete"
                  >删除</el-button
                >
              </el-col>
            </el-row>
          </div>
          <div>
            <el-table
            
              :data="leakData"
              ref="dpTable"
              row-key="id"
              @current-change="handleSelectiondpChange"
              border
              fit
              stripe
              highlight-current-row
              align="left"
            >
              <el-table-column
                property="parameterName"
                label="参数名称"
                  align="center"
                width="160"
              ></el-table-column>
              <el-table-column
                property="leakTimes"
                label="充气时长"
                  align="center"
                width="160"
              ></el-table-column>
              <el-table-column
                property="keepTimes"
                label="保持时长"
                  align="center"
                width="160"
              ></el-table-column>
              <el-table-column
                property="leakPress"
                label="充气压力"
                  align="center"
                width="160"
              ></el-table-column>
              <el-table-column
                property="keepPress"
                label="保持压力"
                  align="center"
                width="160"
              ></el-table-column>
              <el-table-column
                property="upMesCodePN"
                  align="center"
                label="上传MES代码"
              ></el-table-column>
           
                <el-table-column
                property="remark"
                  align="center"
                label="备注"
              ></el-table-column>
            </el-table>
          </div>
        </el-card>
      </el-col>
    </div>
    <!-- </el-dialog> -->
    <!-- 电批弹框1-->
    <!-- 电批弹框2-->
    <el-dialog
      v-el-drag-dialog
      class="dialog-mini"
      width="600px"
      :title="textMap[dialogStatus]"
      :visible.sync="dialogBNVisible"
    >
      <div>
        <el-form
          :rules="anyloadRules"
          ref="dpForm"
          :model="leakTemp"
          label-position="right"
          label-width="100px"
        >
        <!-- <el-form-item size="small" :label="'名称'" prop="name">
            <el-input
              v-model="glueTemp.name"
              v-bind:disabled="dialogStatus != 'create'"
            ></el-input>
          </el-form-item> -->
          <el-form-item size="small" :label="'参数名称'" prop="parameterName">
            <el-input
              v-model="leakTemp.parameterName"
             
            ></el-input>
           
          </el-form-item>
          <el-form-item size="small" :label="'充气时长'" prop="leakTimes">
            <el-input
              v-model="leakTemp.leakTimes"
            ></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'保持时长'" prop="keepTimes">
            <el-input
              v-model="leakTemp.keepTimes"
            ></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'充气压力'" prop="keepTimes">
            <el-input
              v-model="leakTemp.leakPress"
            ></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'保持压力'" prop="keepTimes">
            <el-input
              v-model="leakTemp.keepPress"
            ></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'上传MES代码'" prop="upMesCodePN">
            <el-input v-model="leakTemp.upMesCodePN"></el-input>
          </el-form-item>

          
                 <el-form-item size="small" :label="'备注'" prop="remark">
            <el-input v-model="leakTemp.remark"></el-input>
          </el-form-item>
         
        </el-form>
      </div>
      <div slot="footer">
        <el-button size="mini" @click="dialogBNVisible = false">取消</el-button>
        <el-button
          size="mini"
          v-if="dialogStatus == 'create'"
          type="primary"
          @click="createDpData"
          >确认</el-button
        >
        <el-button size="mini" v-else type="primary" @click="updateDpData"
          >确认</el-button
        >
      </div>
    </el-dialog>
    <!-- 电批弹框2-->
  </div>
</template>

<script>
import * as stationTaskLeak from "@/api/stationTaskLeak.js";
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
      leakTemp: {
        id: undefined,
        programNo: "",
        programNo1:"",
        taoTongNo:"",
     
      },
      textMap: {
        update: "编辑",
        create: "添加",
        detail: "任务详情",
      },
   
   
      
      anyloadRules: {
        //编辑框输入限制
        useNum: [
          {
            required: true,
            message: "使用数量不能为空",
            trigger: "blur",
            
          },
        ],
        deviceNoList: [
          {
            required: true,
            message: "枪号不能为空",
            trigger: "blur",
          
          },
        ],
        screwSpecs:[
          {
            required: true,
            message: "螺丝规格不能为空",
            trigger: "blur",
           
          },
        ],
       
      },
      
      dialogStatus: "", //编辑框功能(添加/编辑)
      leakData: [],
     
      dialogBNVisible: false,
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
      this.leakTemp = {
        id: undefined,
        screwSpecs: "",
        programNo: "",
        useNum: 1,
        remark: "",
        name:"",
     
        taoTongNo:"",
        stationTaskId: this.taskId,
        reworkLimitTimes:0,
        torqueMinLimit:"",
        torqueMaxLimit:"",
        angleMinLimit:"",
        angleMaxLimit:"",
        upMesCodePN:"",
      };
    },

    handledpAdd() {
      this.dialogStatus = "create"; //编辑框功能选择（添加）
      this.dialogBNVisible = true; //编辑框显示
      this.$nextTick(() => {
        this.$refs["dpForm"].clearValidate();
      });
      this.resetdpdata();
    },
    handledpEdit() {
      if (this.leakTemp.id != undefined) {
        this.dialogStatus = "update"; //编辑框功能选择（添加）
        this.dialogBNVisible = true; //编辑框显示
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
      if (this.leakTemp.id === undefined) {
        this.$message({
          message: "请选择一个想要删除的数据",
          type: "error",
        });
        return;
      }
      this.$confirm("确定要删除吗？").then((_) => {
        //提取复选框的数据的Id
        var selectids = [];
        selectids.push(this.leakTemp.id); //提取复选框的数据的Id
        var params = {
          ids: selectids,
        };
        stationTaskLeak.del(params).then(() => {
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
        this.leakTemp = val;
      }
    },

    updateDpData() {
      this.$refs["dpForm"].validate((valid) => {
        if (valid) {
          stationTaskLeak.update(this.leakTemp).then((response) => {
            this.dialogBNVisible = false; //编辑框关闭
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
          stationTaskLeak.add(this.leakTemp).then((response) => {
            this.dialogBNVisible = false; //编辑框关闭
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
      stationTaskLeak.GetByTaskId({ taskid: this.taskId }).then((response) => {
        this.leakData = response.result; //提取数据表
        console.log(this.leakData);
      });
      this.$nextTick(() => {});
    },
   
    //#endregion
    back() {
      this.$parent.leakVisible = false;
      this.$parent.taskvisible = true;
      this.taskId = 0;
      this.stepId = 0;
    },
  },
};
</script>
 
<style>
</style>