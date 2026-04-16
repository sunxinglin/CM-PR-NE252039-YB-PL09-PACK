<template>
  <div>
    <div class="app-container">
      <el-col :span="24">
        <el-card shadow="never" class="boby-small" style="height: 100%">
          <div slot="header" class="clearfix">
            <span>自动涂胶任务详情</span>
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
            <el-table :data="glueData" ref="dpTable" row-key="id" :height="tableHeight"
              @current-change="handleSelectiondpChange" border fit stripe highlight-current-row align="left">
              <el-table-column property="parameterName" label="参数名称" align="center" width="160"></el-table-column>
              <el-table-column prop="glueType" label="涂胶参数类型" :formatter="setGluetype" min-width="60px" sortable align="center"></el-table-column>
              <el-table-column property="glueLocate" align="center" label="涂胶位置"></el-table-column>
              <el-table-column property="minValue" align="center" label="最小值"></el-table-column>
              <el-table-column property="maxValue" align="center" label="最大值"></el-table-column>
              <el-table-column property="upMesCode" align="center" label="上传MES代码"></el-table-column>
            </el-table>
          </div>
        </el-card>
      </el-col>
    </div>
    <!-- </el-dialog> -->
    <!-- 电批弹框1-->
    <!-- 电批弹框2-->
    <el-dialog v-el-drag-dialog class="dialog-mini" width="600px" :title="textMap[dialogStatus]"
      :visible.sync="dialogGlueVisible">
      <div>
        <el-form :rules="glueRules" ref="dpForm" :model="glueTemp" label-position="right" label-width="100px">
          <el-form-item size="small" :label="'参数名称'" prop="parameterName">
            <el-input v-model="glueTemp.parameterName"></el-input>
          </el-form-item>

          <el-form-item size="small" :label="'涂胶参数类型'" prop="glueType">
              <el-select v-model="glueTemp.glueType" placeholder="请选择">
                <el-option v-for="item in typeoptions" :key="item.id" :label="item.name" :value="item.id">
                </el-option>
              </el-select>
            </el-form-item>

          <el-form-item size="small" :label="'涂胶位置'" prop="parameterName">
            <el-input v-model="glueTemp.glueLocate"></el-input>
          </el-form-item>

          <el-form-item size="small" :label="'最小值'" prop="parameterName">
            <el-input v-model="glueTemp.minValue"></el-input>
          </el-form-item>

          <el-form-item size="small" :label="'最大值'" prop="parameterName">
            <el-input v-model="glueTemp.maxValue"></el-input>
          </el-form-item>

          <el-form-item size="small" :label="'上传MES代码'" prop="upMesCode">
            <el-input v-model="glueTemp.upMesCode" ></el-input>
          </el-form-item>
        </el-form>
      </div>
      <div slot="footer">
        <el-button size="mini" @click="dialogGlueVisible = false">取消</el-button>
        <el-button size="mini" v-if="dialogStatus == 'create'" type="primary" @click="createDpData">确认</el-button>
        <el-button size="mini" v-else type="primary" @click="updateDpData">确认</el-button>
      </div>
    </el-dialog>
    <!-- 电批弹框2-->
  </div>
</template>

<script>
import * as stationTaskAutoGlue from "@/api/stationTaskAutoGlue.js";
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
      tableHeight: 500,
      glueTemp : {
        id: undefined,
        parameterName:"",
        glueType: 1,
        stationTaskId: this.taskId,
        glueLocate:1,
        minValue:0,
        maxValue:999,
        upMesCode: "",
      },
      textMap: {
        update: "编辑",
        create: "添加",
        detail: "任务详情",
      },

      typeoptions: [
        {
          id:1,
          name:"A胶"
        },
        {
          id:2,
          name:"B胶"
        },
        {
          id:3,
          name:"胶比例"
        },
        {
          id:4,
          name:"总胶重"
        },
      ],

      glueRules: {
        //编辑框输入限制
        glueType: [
          {
            required: true,
            message: "涂胶类型不能为空",
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
        upMesCode: [
          {
            required: true,
            message: "上传代码不能为空",
            trigger: "blur",

          },
        ],
      },
      dialogStatus: "", //编辑框功能(添加/编辑)
      glueData: [],
      dialogGlueVisible: false,
      taskId: 0,
      stepId: 0,
    };
  },
  mounted() {
    this.Load();

    this.$nextTick(() => {
      const table = this.$refs.dpTable;
      if (!table || !table.$el) return;
      const h = document.documentElement.clientHeight;
      const topH = table.$el.offsetTop;
      const tableHeight = Math.floor((h - topH) * 0.7);
      this.tableHeight = tableHeight >= 200 ? tableHeight : 200;
    });
  },
  methods: {
    resetdpdata() {
      this.$refs.dpTable.setCurrentRow();
      this.glueTemp = {
        id: undefined,
        parameterName:"",
        glueType: 1,
        stationTaskId: this.taskId,
        glueLocate:1,
        minValue:0,
        maxValue:999,
        upMesCode: "",
      };
    },
    setGluetype(row, column, cellValue) {
      switch (cellValue) {
        case 1:
          return "A胶";
        case 2:
          return "B胶";
        case 3:
          return "胶比例";
        case 4:
          return "总胶重";
        default:
          return null;
      }
    },
    handledpAdd() {

      this.dialogStatus = "create"; //编辑框功能选择（添加）
      this.dialogGlueVisible = true; //编辑框显示
      this.$nextTick(() => {
        this.$refs["dpForm"].clearValidate();
      });
      this.resetdpdata();
    },
    handledpEdit() {
      if (this.glueTemp.id != undefined) {
        this.dialogStatus = "update"; //编辑框功能选择（添加）
        this.dialogGlueVisible = true; //编辑框显示
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
            this.dialogGlueVisible = false; //编辑框关闭
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
            this.dialogGlueVisible = false; //编辑框关闭
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
      stationTaskAutoGlue.GetByTaskId({ taskid: this.taskId }).then((response) => {
        this.glueData = response.result; //提取数据表
      });
      this.$nextTick(() => { });
    },

    //#endregion
    back() {
      this.$parent.gluevisible = false;
      this.$parent.taskvisible = true;
      this.taskId = 0;
      this.stepId = 0;
    },
  },
};
</script>
 
<style></style>
