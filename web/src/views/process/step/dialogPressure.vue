<template>
  <div>
    <div class="app-container">
      <el-col :span="24">
        <el-card shadow="never" class="boby-small" style="height: 100%">
          <div slot="header" class="clearfix">
            <span>自动加压任务详情</span>
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
            <el-table :data="pressureData" ref="dpTable" row-key="id" @current-change="handleSelectiondpChange" border
              fit stripe highlight-current-row align="left">
              <el-table-column property="parameterName" label="参数名称" align="center"></el-table-column>
              <el-table-column property="pressureLocate" label="加压位置" align="center"></el-table-column>
              <el-table-column prop="pressurizeDataType" label="数据类型" :formatter="setDatatype" min-width="60px" sortable
                align="center"></el-table-column>
              <el-table-column property="minValue" align="center" label="最小值"></el-table-column>
              <el-table-column property="maxValue" align="center" label="最大值"></el-table-column>
              <el-table-column property="upMesCode" align="center" label="上传MES代码"></el-table-column>
            </el-table>
          </div>
        </el-card>
      </el-col>
    </div>

    <el-dialog v-el-drag-dialog class="dialog-mini" width="600px" :title="textMap[dialogStatus]"
      :visible.sync="dialogPressureVisible">
      <div>
        <el-form :rules="pressureRules" ref="dpForm" :model="pressureTemp" label-position="right" label-width="100px">
          <el-form-item size="small" :label="'参数名称'" prop="parameterName">
            <el-input v-model="pressureTemp.parameterName"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'加压位置'" prop="parameterName">
            <el-input v-model="pressureTemp.pressureLocate"></el-input>
          </el-form-item>

          <el-form-item size="small" :label="'数据类型'" prop="glueType">
              <el-select v-model="pressureTemp.pressurizeDataType" placeholder="请选择">
                <el-option v-for="item in typeoptions" :key="item.id" :label="item.name" :value="item.id">
                </el-option>
              </el-select>
            </el-form-item>
          <el-form-item size="small" :label="'最小值'">
            <el-input v-model="pressureTemp.minValue"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'最大值'">
            <el-input v-model="pressureTemp.maxValue"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'上传MES代码'" prop="upMesCode">
            <el-input v-model="pressureTemp.upMesCode"></el-input>
          </el-form-item>
        </el-form>
      </div>
      <div slot="footer">
        <el-button size="mini" @click="dialogPressureVisible = false">取消</el-button>
        <el-button size="mini" v-if="dialogStatus == 'create'" type="primary" @click="createDpData">确认</el-button>
        <el-button size="mini" v-else type="primary" @click="updateDpData">确认</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import * as stationTaskPressure from "@/api/stationTaskPressure.js";
import elDragDialog from "@/directive/el-dragDialog";
import waves from "@/directive/waves"; // 水波纹指令
export default {
  directives: {
    waves,
    elDragDialog,
  },
  data() {
    return {
      pressureTemp: {
        id: undefined,
        stationTaskId: this.taskId,
        parameterName: "",
        pressureLocate: 1,
        minValue: 0,
        maxValue: 999,
        pressurizeDataType:1,
        upMesCode: "",
      },
      textMap: {
        update: "编辑",
        create: "添加",
        detail: "任务详情",
      },

      pressureRules: {
        upMesCode: [
          {
            require: true,
            message: "上传代码不能为空",
            trigger: "blur",
          },
        ],
      },
      typeoptions: [
        {
          id: 0,
          name: "肩部高度",
        },
        {
          id: 1,
          name: "保压时长",
        },
        {
          id: 2,
          name: "平均压力",
        },
        {
          id: 3,
          name: "最大压力",
        },
      ],

      dialogStatus: "", //编辑框功能(添加/编辑)
      pressureData: [],

      dialogPressureVisible: false,
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
      this.pressureTemp = {
        id: undefined,
        stationTaskId: this.taskId,
        parameterName: "",
        pressureLocate: 1,
        pressurizeDataType:1,
        minValue: 0,
        maxValue: 999,
        upMesCode: "",
      };
    },
    setDatatype(row, column, cellValue) {
      switch (cellValue) {
        case 0:
          return "肩部高度";
        case 1:
          return "保压时长";
        case 2:
          return "平均压力";
        case 3:
          return "最大压力";
        default:
          return null;
      }
    },
    handledpAdd() {
      this.dialogStatus = "create"; //编辑框功能选择（添加）
      this.dialogPressureVisible = true; //编辑框显示
      this.$nextTick(() => {
        this.$refs["dpForm"].clearValidate();
      });
      this.resetdpdata();
    },
    handledpEdit() {
      if (this.pressureTemp.id != undefined) {
        this.dialogStatus = "update"; //编辑框功能选择（添加）
        this.dialogPressureVisible = true; //编辑框显示
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
      if (this.pressureTemp.id === undefined) {
        this.$message({
          message: "请选择一个想要删除的数据",
          type: "error",
        });
        return;
      }
      this.$confirm("确定要删除吗？").then((_) => {
        //提取复选框的数据的Id
        var selectids = [];
        selectids.push(this.pressureTemp.id); //提取复选框的数据的Id
        var params = {
          ids: selectids,
        };
        stationTaskPressure.del(params).then(() => {
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
        this.pressureTemp = val;
      }
    },

    updateDpData() {
      this.$refs["dpForm"].validate((valid) => {
        if (valid) {
          stationTaskPressure.update(this.pressureTemp).then((response) => {
            this.dialogPressureVisible = false; //编辑框关闭
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
          stationTaskPressure.add(this.pressureTemp).then((response) => {
            this.dialogPressureVisible = false; //编辑框关闭
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
      stationTaskPressure
        .GetByTaskId({ taskid: this.taskId })
        .then((response) => {
          this.pressureData = response.result; //提取数据表
          console.log(this.pressureData);
        });
      this.$nextTick(() => { });
    },

    //#endregion
    back() {
      this.$parent.pressurevisible = false;
      this.$parent.taskvisible = true;
      this.taskId = 0;
      this.stepId = 0;
    },
  },
};
</script>

<style></style>
