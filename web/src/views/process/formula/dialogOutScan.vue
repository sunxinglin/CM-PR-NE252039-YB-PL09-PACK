<template>
  <div>
    <div class="app-container">
      <el-col :span="24">
        <el-card shadow="never" class="boby-small" style="height: 100%">
          <div slot="header" class="clearfix">
            <span>用户输入任务详情</span>
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
            <el-table :data="scandata" ref="dpTable" row-key="id" @row-click="weightrowclick"
              @current-change="handleSelectionweightChange" border fit stripe highlight-current-row align="left">
              <el-table-column property="userScanName" label="详情名称" align="center" width="160"></el-table-column>
              <el-table-column property="maxRange" label="最大值" align="center"></el-table-column>
              <el-table-column property="minRange" label="最小值" align="center"></el-table-column>
              <!-- <el-table-column property="needValidate" :formatter="formatterBoolean" align="center" label="是否需要服务器校验">
              </el-table-column> -->

              <el-table-column property="upMesCode" label="上传代码" align="center"></el-table-column>

            </el-table>
          </div>

        </el-card>
      </el-col>
    </div>
    <!-- </el-dialog> -->
    <!-- 外部输入列表结束 -->
    <!-- 外部输入表单开始 -->
    <el-dialog v-el-drag-dialog class="dialog-mini" width="600px" :title="textMap[dialogStatus]"
      :visible.sync="dialogoutscanVisible">
      <div>
        <el-form :rules="weightRules" ref="weightForm" :model="scan" label-position="right" label-width="100px">
          <el-form-item size="small" :label="'详情名称'">
            <el-input v-model="scan.userScanName"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'最小值'" prop="minNum">
            <el-input type="number" v-model="scan.minRange"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'最大值'" prop="maxNum">
            <el-input type="number" v-model="scan.maxRange"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'上传代码'" prop="upMesCode">
            <el-input v-model="scan.upMesCode"></el-input>
          </el-form-item>
          <!-- <el-form-item size="small" :label="'是否MES校验'" label-width="150px">
            <el-switch v-model="scan.needValidate"></el-switch>
          </el-form-item> -->
        </el-form>
      </div>
      <div slot="footer">
        <el-button size="mini" @click="dialogoutscanVisible = false">取消</el-button>
        <el-button size="mini" v-if="dialogStatus == 'create'" type="primary" @click="createWeightData">确认</el-button>
        <el-button size="mini" v-else type="primary" @click="updateWeightData">确认</el-button>
      </div>
    </el-dialog>
    <!-- 外部输入表单结束 -->
  </div>
</template>

<script>
import * as stationTaskOutScan from "@/api/stationTaskOutScan";
import elDragDialog from "@/directive/el-dragDialog";
import waves from "@/directive/waves"; // 水波纹指令
export default {
  directives: {
    waves,
    elDragDialog,
  },
  data() {
    var validateMinWeight = (rule, value, callback) => {
      if (value > this.scan.maxNum) {
        callback(new Error("最小值不能大于最大值"));
      } else {
        callback();
      }
    };
    var validateMaxWeight = (rule, value, callback) => {
      if (value < this.scan.minNum) {
        callback(new Error("最大值不能大于最小值"));
      } else {
        callback();
      }
    };
    return {
      textMap: {
        update: "编辑",
        create: "添加",
        detail: "任务详情",
      },
      dialogoutscanVisible: false,

      scanTemp: {},
      scan: {
        userScanName: "",
        maxRange: 0,
        minRange: 0,
        needValidate: true,
        stationTaskId: 0,
        upMesCode: ''
      },
      weightRules: {
        maxNum: [
          {
            required: true,
            message: "最大值不能为空",
            trigger: "blur",
            validator: validateMaxWeight,
          },
        ],
        minNum: [
          {
            required: true,
            message: "最小值不能为空",
            trigger: "blur",
            validator: validateMinWeight,
          },
        ],
        anyLoadName: [
          {
            required: true,
            message: "名称不能为空",
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
      scandata: [],
      taskId: 0,
    };
  },
  mounted() {
    this.reloadWeightData();
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

    resetweightdata() {
      this.scan = {
        id: undefined,
        maxRange: "",
        minRange: "",
        stationTaskId: this.taskId,
      };
    },

    handleWeightAdd() {
      if (this.scandata.length>=1) {
        this.$message({
          message: "只可添加一个任务",
          type: "error",
        });
        return;
      }
      this.dialogStatus = "create"; //编辑框功能选择（添加）
      this.dialogoutscanVisible = true; //编辑框显示
      this.$nextTick(() => {
        this.$refs["weightForm"].clearValidate();
      });
      this.resetweightdata();
    },
    handleWeightEdit() {
      if (this.scanTemp.id != undefined) {
        this.dialogStatus = "update"; //编辑框功能选择（添加）
        this.dialogoutscanVisible = true; //编辑框显示
        this.$nextTick(() => {
          this.$refs["weightForm"].clearValidate();
        });
        this.resetweightdata();
        this.scan = this.scanTemp;
      } else {
        this.$message({
          message: "请选择一个想要修改的数据",
          type: "error",
        });
      }
    },
    handleWeightDelete() {
      if (this.scanTemp.id === undefined) {
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
          selectids.push(this.scanTemp.id); //提取复选框的数据的Id
          var params = {
            ids: selectids,
          };
          stationTaskOutScan.del(params).then(() => {
            this.$notify({
              title: "成功",
              message: "删除成功",
              type: "success",
              duration: 2000,
            });
            this.reloadWeightData();
            //页面加载
          });
        })
        .catch((_) => { });
    },
    handleSelectionweightChange(val) {
      if (val === null) {
        return;
      } else {
        this.scanTemp = val;
      }
    },
    weightrowclick(row) {
      this.scanTemp = row;
    },
    updateWeightData() {
      this.$refs["weightForm"].validate((valid) => {
        if (valid) {
          stationTaskOutScan.update(this.scan).then((response) => {
            this.dialogoutscanVisible = false; //编辑框关闭
            this.$notify({
              title: "成功",
              message: "修改成功",
              type: "success",
              duration: 2000,
            });
            this.resetweightdata();
            this.reloadWeightData();
          });
        }

      });
    },
    createWeightData() {
      this.$refs["weightForm"].validate((valid) => {
        if (valid) {
          console.log(this.scan);

          stationTaskOutScan.add(this.scan).then((response) => {
            this.dialogoutscanVisible = false; //编辑框关闭
            this.$notify({
              title: "成功",
              message: "创建成功",
              type: "success",
              duration: 2000,
            });
            this.resetweightdata();
            this.reloadWeightData();
          });
        }
      });
    },
    reloadWeightData() {

      if (this.taskId == 0) {
        this.taskId = this.$parent.taskId;
      }
      console.log(this.taskId);
      stationTaskOutScan.load({ taskid: this.taskId }).then((response) => {
        this.scandata = response.data; //提取数据表
      });
      this.$nextTick(() => { });
    },

    //#endregion
    back() {
      this.taskId = 0;
      this.$parent.outscanvisiable = false;
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

<style>

</style>