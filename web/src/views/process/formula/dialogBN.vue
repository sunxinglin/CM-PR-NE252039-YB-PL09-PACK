<template>
  <div>
    <div class="app-container">
      <el-col :span="24">
        <el-card shadow="never" class="boby-small" style="height: 100%">
          <div slot="header" class="clearfix">
            <span>补拧任务详情</span>
          </div>
          <div style="margin-bottom: 10px">
            <el-row :gutter="4">
              <el-col :span="20">
                <el-button type="warning" icon="el-icon-back" size="mini" @click="back">返回</el-button>
                <el-button type="primary" icon="el-icon-plus" size="mini" @click="handledpAdd">添加</el-button>
                <el-button type="primary" icon="el-icon-edit" size="mini" @click="handledpEdit">编辑</el-button>
                <el-button type="primary" icon="el-icon-delete" size="mini" @click="handledpDelete">删除</el-button>
                <el-button type="text" size="mini" @click="showAdvanced = !showAdvanced">
                  {{ showAdvanced ? "隐藏高级参数" : "显示高级参数" }}
                </el-button>
              </el-col>
            </el-row>
          </div>
          <div>
            <el-table :data="dpData" ref="dpTable" row-key="id" @current-change="handleSelectiondpChange" border fit
              stripe highlight-current-row align="left">
              <el-table-column property="taskName" label="任务名称" align="center" width="160"></el-table-column>
              <el-table-column property="reworkType" label="返工类型" :formatter="setTightenReworktype" align="center" ></el-table-column>
              <el-table-column property="screwNum" align="center" label="螺栓总量"></el-table-column>
              <el-table-column property="programNo" align="center" label="程序号"></el-table-column>
              <el-table-column property="devicesNos" label="枪号" align="center"></el-table-column>
              <template v-if="showAdvanced">
                <el-table-column property="minTorque" label="最小扭矩" align="center"></el-table-column>
                <el-table-column property="maxTorque" label="最大扭矩" align="center"></el-table-column>
                <el-table-column property="minAngle" label="最小角度" align="center"></el-table-column>
                <el-table-column property="maxAngle" label="最大角度" align="center"></el-table-column>
              </template>
              <el-table-column property="upMesCode" label="上传代码" align="center"></el-table-column>
            </el-table>
          </div>
        </el-card>
      </el-col>
    </div>
    <el-dialog v-el-drag-dialog class="dialog-mini" width="600px" :title="textMap[dialogStatus]"
      :visible.sync="dialogBNVisible">
      <div>
        <el-form :rules="anyloadRules" ref="dpForm" :model="dpTemp" label-position="right" label-width="100px">
       
          <el-form-item size="small" :label="'任务名称'" prop="screwSpecs">
            <el-input v-model="dpTemp.taskName"></el-input>
          </el-form-item>
        
          <el-form-item size="small" :label="'返工类型'" prop="reworkType">
            <el-select v-model="dpTemp.reworkType" placeholder="请选择" style="width: 100%" >
              <el-option v-for="item in typeoptions" :key="item.id" :label="item.name" :value="item.id">
              </el-option>
            </el-select>
          </el-form-item>

          <el-form-item size="small" :label="'螺栓总量'" prop="screwNum">
            <el-input v-model="dpTemp.screwNum"></el-input>
          </el-form-item>

          <el-form-item size="small" :label="'程序号'" prop="programNo">
            <el-input v-model="dpTemp.programNo"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'枪号'" prop="devicesNos">
            <el-input v-model="dpTemp.devicesNos"></el-input>
          </el-form-item>
          <el-collapse v-model="editCollapseActive">
            <el-collapse-item title="高级参数" name="advanced">
              <el-form-item size="small" :label="'最小扭矩'" prop="torqueMinLimit">
                <el-input v-model="dpTemp.minTorque"></el-input>
              </el-form-item>
              <el-form-item size="small" :label="'最大扭矩'" prop="torqueMaxLimit">
                <el-input v-model="dpTemp.maxTorque"></el-input>
              </el-form-item>
              <el-form-item size="small" :label="'最小角度'" prop="angleMinLimit">
                <el-input v-model="dpTemp.minAngle"></el-input>
              </el-form-item>
              <el-form-item size="small" :label="'最大角度'" prop="angleMaxLimit">
                <el-input v-model="dpTemp.maxAngle"></el-input>
              </el-form-item>
            </el-collapse-item>
          </el-collapse>
          <el-form-item size="small" :label="'上传代码'" prop="upMesCode">
            <el-input v-model="dpTemp.upMesCode"></el-input>
          </el-form-item>
        </el-form>
      </div>
      <div slot="footer">
        <el-button size="mini" @click="dialogBNVisible = false">取消</el-button>
        <el-button size="mini" v-if="dialogStatus == 'create'" type="primary" @click="createDpData">确认</el-button>
        <el-button size="mini" v-else type="primary" @click="updateDpData">确认</el-button>
      </div>
    </el-dialog>
    <!-- 电批弹框2-->
  </div>
</template>

<script>
import * as stationtaskscrewrework from "@/api/stationtaskscrewrework.js";
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
      if (rule.field =="useNum") {
        if (Number(value)  < 1) {
        callback(new Error("最小为1"));
      } 
      }
      if (Number(value)  < 0) {
        callback(new Error("最小为0"));
      }

    
      callback();
    };
    return {
      showAdvanced: false,
      editCollapseActive: [],
      dpTemp: {
        id: undefined,
        stationTaskId:this.taskId,
        taskName:'',
        reworkType:1,
        screwNum:1,
        programNo:1,
        upMesCode:"",
        devicesNos:"1",
        minTorque:0,
        maxTorque:180,
        minAngle:0,
        maxAngle:180,
      },
      textMap: {
        update: "编辑",
        create: "添加",
        detail: "任务详情",
      },
      stationBoltGunList: [], //拧紧枪列表
      stationBoltGunGroupList: [], //拧紧枪列表
      typeoptions: [
        {
          id:0,
          name:"模组补拧"
        },
        {
          id:1,
          name:"上盖补拧"
        },
        {
          id:2,
          name:"压条补拧"
        },
      ],
      anyloadRules: {
        //编辑框输入限制
        reworkType: [
          {
            required: true,
            message: "使用数量不能为空",
            trigger: "blur",
            validator: checkPositivenumber,
          },
        ],
        screwNum: [
          {
            required: true,
            message: "使用数量不能为空",
            trigger: "blur",
            validator: checkPositivenumber,
          },
        ],
        devicesNos: [
          {
            required: true,
            message: "枪号不能为空",
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
        upMesCode: [
          {
            required: true,
            message: "上传代码不能为空",
            trigger: "blur",
          },
        ],
      },
      
      dialogStatus: "", //编辑框功能(添加/编辑)
      dpData: [],
      resourceData: [],
      dialogBNVisible: false,
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
      this.$refs.dpTable.setCurrentRow();
      this.dpTemp = {
        id: undefined,
        taskName:'',
        stationTaskId:this.taskId,
        reworkType:1,
        screwNum:1,
        programNo:1,
        upMesCode:"",
        devicesNos:"1",
        minTorque:0,
        maxTorque:180,
        minAngle:0,
        maxAngle:180,
      };
    },
    setTightenReworktype(row, column, cellValue) {
      switch (cellValue) {
        case 0:
          return "模组补拧";
        case 1:
          return "上盖补拧";
        case 2:
          return "压条补拧";
        default:
          return null;
      }
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
      if (this.dpTemp.id != undefined) {
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
        stationtaskscrewrework.del(params).then(() => {
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
          this.dpTemp.reworkLimitTimes=this.dpTemp.useNum;
          stationtaskscrewrework.update(this.dpTemp).then((response) => {
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
          this.dpTemp.stationTaskId = this.taskId;
          stationtaskscrewrework.add(this.dpTemp).then((response) => {
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
      stationtaskscrewrework.Load({ taskid: this.taskId }).then((response) => {
        this.dpData = response.result; //提取数据表
      });
      this.$nextTick(() => {});
    },
    loadresource() {
      proresource
        .getlistbystepId({ stepId: this.stepId })
        .then((response) => {
          this.resourceData = response.result;
          console.log(this.resourceData);
        });
    },
    back() {
      this.$parent.BNvisible = false;
      this.$parent.taskvisible = true;
      this.taskId = 0;
      this.stepId = 0;
    },
  },
};
</script>
 
<style>
</style>
