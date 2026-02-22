<template>
  <div>
    <div class="app-container">
      <el-col :span="24">
        <el-card shadow="never" class="boby-small" style="height: 100%">
          <div slot="header" class="clearfix">
            <span>肩部涂胶首件配置详情</span>
          </div>
          <div style="margin-bottom: 10px">
            <el-row :gutter="3">
              <el-col :span="20">
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
                  type="danger"
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
            
              :data="glueData"
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
                property="upMesCodePN"
                  align="center"
                label="上传MES代码"
              ></el-table-column>
              <!-- <el-table-column
                property="programNo"
                  align="center"
                label="程序号"
              ></el-table-column> -->
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
          :model="glueTemp"
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
              v-model="glueTemp.parameterName"
             
            ></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'上传MES代码'" prop="upMesCodePN">
            <el-input v-model="glueTemp.upMesCodePN"></el-input>
          </el-form-item>

         
                 <el-form-item size="small" :label="'备注'" prop="remark">
            <el-input v-model="glueTemp.remark"></el-input>
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
import * as stationTaskAutoGlue from "@/api/stationGlueShouJian.js";
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
      glueTemp: {
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
      glueData: [],
     
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
      this.glueTemp = {
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
        glueStationCode:"OP240",
        ShouJianCommUploadType:3
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
      if (this.glueTemp.id != undefined) {
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
      if (this.glueTemp.id === undefined) {
        this.$message({
          message: "请选择一个想要删除的数据",
          type: "error",
        });
        return;
      }
      this.$confirm("确定要删除吗？").then((_) => {
        //提取复选框的数据的Id
        var selectids = [];
        selectids.push(this.glueTemp.id); //提取复选框的数据的Id
        var params = {
          ids: selectids,
        };
        stationTaskAutoGlue.del(params).then(() => {
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
        this.glueTemp = val;
      }
    },

    updateDpData() {
      this.$refs["dpForm"].validate((valid) => {
        if (valid) {
          stationTaskAutoGlue.update(this.glueTemp).then((response) => {
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
          stationTaskAutoGlue.add(this.glueTemp).then((response) => {
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
      stationTaskAutoGlue.GetByTaskId({ type:3}).then((response) => {
        this.glueData = response.result; //提取数据表
        console.log(this.glueData);
      });
      this.$nextTick(() => {});
    },
   
    //#endregion
    back() {
      this.$parent.Gluevisible = false;
      this.$parent.taskvisible = true;
      this.taskId = 0;
      this.stepId = 0;
    },
  },
};
</script>
 
<style>
</style>