<template>
  <div style="height:100% ">
    <div class="app-container " style="height: 100%;" v-if="taskvisible">
      <div class="filter-container  ">
        <el-card shadow="never" class="boby-small" style="height: 100%">
          <div slot="header" class="clearfix">
            <span>{{ stepname }}</span>
          </div>
          <div>
            <el-row :gutter="2">
              <el-col :span="21">
                <el-button type="warning" icon="el-icon-back" size="mini" @click="back">返回
                </el-button>
                <el-button type="primary" icon="el-icon-plus" size="mini" @click="handleCreateTask">
                  添加</el-button>
                <el-button type="primary" icon="el-icon-edit" size="mini" @click="handleUpdateTask">
                  编辑</el-button>

                <el-button type="primary" icon="el-icon-more" size="mini" @click="handleDetail">任务详情
                </el-button>
              </el-col>
            </el-row>
          </div>
        </el-card>
      </div>
      <div class="app-container" style="height: calc(100%);">
        <el-table ref="taskTable" :data="stationTakList" style="width: 100%" height="100% "
          @current-change="handleSelectionStationTaskChange" border fit stripe highlight-current-row align="left">
          <el-table-column prop="sequence" label="任务顺序号" min-width="40px" sortable align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.sequence }}</span>
            </template>
          </el-table-column>
          <el-table-column prop="name" label="名称" min-width="60px" sortable align="center">
          </el-table-column>
          <el-table-column prop="type" label="类型" :formatter="setstationTaketype" min-width="60px" sortable
            align="center"></el-table-column>
          <!-- <el-table-column
            prop="hasPage"
            label="是否有页面"
            :formatter="formatterBoolean"
            min-width="60px"
            sortable
            align="center"
          ></el-table-column> -->

          <el-table-column prop="description" label="描述" min-width="60px" sortable align="center">
          </el-table-column>
          <el-table-column prop="sequence" label="操作顺序" min-width="60px" sortable align="center">
            <template slot-scope="scope">
              <el-button-group>
                <el-button size="small" plain type="success" icon="el-icon-top" @click.native.prevent="
                  handleDetailUpDown(scope.row.id, scope.$index, -1, $event)
                  "></el-button>
                <el-button size="small" plain type="success" icon="el-icon-bottom" @click.native.prevent="
                  handleDetailUpDown(scope.row.id, scope.$index, 1, $event)
                  "></el-button>
                <el-button size="small" plain type="danger" icon="el-icon-delete"
                  @click.native.prevent="DeleteTask(scope.row)"></el-button>
              </el-button-group>
            </template>
          </el-table-column>
        </el-table>

      </div>

      <el-dialog v-el-drag-dialog class="dialog-mini" width="500px" :title="textMap[dialogStatus]"
        :visible.sync="dialogStionTaskVisible">
        <div>
          <el-form :rules="stepRules" ref="taskForm" :model="taskTemp" label-position="right" label-width="100px">
            <!-- <el-form-item size="small" :label="'编码'" prop="code">
              <el-input
                v-model="taskTemp.code"
                v-bind:disabled="dialogStatus != 'create'"
              ></el-input>
            </el-form-item> -->
            <el-form-item size="small" :label="'名称'" prop="name">
              <el-input v-model="taskTemp.name"></el-input>
            </el-form-item>

            <el-form-item size="small" :label="'类型'">
              <el-select v-model="taskTemp.type" placeholder="请选择">
                <el-option v-for="item in typeoptions" :key="item.id" :label="item.name" :value="item.id">
                </el-option>
              </el-select>
            </el-form-item>
            <el-form-item size="small" :label="'操作顺序'" v-show="dialogStatus != 'create'">
              <span> {{ taskTemp.sequence }}</span>
            </el-form-item>
            <el-form-item size="small" :label="'描述'">
              <el-input v-model="taskTemp.description"></el-input>
            </el-form-item>
          </el-form>
        </div>
        <div slot="footer">
          <el-button size="mini" @click="dialogStionTaskVisible = false">取消</el-button>
          <el-button size="mini" v-if="dialogStatus == 'create'" type="primary" @click="createTaskData">确认
          </el-button>
          <el-button size="mini" v-else type="primary" @click="updateTaskData">确认</el-button>
        </div>
      </el-dialog>
    </div>
    <angload v-if="angloadvisible" ref="angload" :taskId="taskId"></angload>
    <DP v-if="Dpvisibled" ref="dp" :taskId="taskId" />
    <bom v-if="bomvisible" ref="bom" :taskId="taskId" />
    <BN v-if="BNvisible" ref="bn" :taskId="taskId" />
    <Timeout v-if="timeoutvisiable" ref="timeout" :taskId="taskId" />
    <OutScan v-if="outscanvisiable" ref="OutScan" :taskId="taskId"></OutScan>
    <ScanCollect v-if="scanCollectVisiable" ref="ScanCollect" :taskId="taskId" />
    <ScanAccountCard v-if="scanAccountCardVisible" ref="ScanAccountCard" :taskId="taskId" />
    <TimeRecord v-if="recordTimeVisible" ref="TimeRecord" :taskId="taskId" />

    <Glue v-if="gluevisible" ref="glue" :taskId="taskId" />
    <AutoTighten v-if="autoTightenVisiable" ref="AutoTighten" :taskId="taskId" />
    <BlockIncase v-if="blockIncaseVisiable" ref="BlockIncase" :taskId="taskId" />
    <Pressure v-if="pressurevisible" ref="Pressure" :taskId="taskId" />
    <LowerBoxGlue v-if="lowerBoxGlueVisible" ref="LowerBoxGlue" :taskId="taskId" />
    <DialogImageTighten v-if="imageTightenVisible" ref="NgUpload" :taskId="taskId" />
    <DialogLeak v-if="leakVisible" ref="DialogLeak" :taskId="taskId" />
  </div>
</template>

<script>
import ScanAccountCard from "./dialogScanAccountCard.vue";
import OutScan from "./dialogOutScan.vue";
import ScanCollect from "./dialogScanCollect";
import Timeout from "./dialogTimeout.vue";
import Angload from "./dialogAngload.vue";
import bom from "./dialogbom.vue";
import DP from "./dialogDP.vue";
import BN from "./dialogBN.vue";
import TimeRecord from "./dialogTimeRecord.vue";

import Glue from "./dialogGlue.vue"
import AutoTighten from "./dialogAutoTighten.vue"
import BlockIncase from "./dialogBlockIncase.vue"
import Pressure from "./dialogPressure.vue"
import LowerBoxGlue from "./dialogLowerBoxGlue.vue"
import DialogImageTighten from "./dialogImageTighten.vue"
import DialogLeak from "./dialogLeak.vue"

import * as axios from "axios";
import waves from "@/directive/waves"; // 水波纹指令
import Sticky from "@/components/Sticky";
import permissionBtn from "@/components/PermissionBtn";
import Pagination from "@/components/Pagination";
import elDragDialog from "@/directive/el-dragDialog";
import * as stationtask from "@/api/stationtask.js";
// import * as stationTaskAuto from "@/api/stationTaskAuto.js";
import { log } from 'console';
export default {
  name: "stationtask",
  components: {
    Sticky,
    permissionBtn,
    Pagination,
    Angload,
    bom,
    DP,
    BN,
    TimeRecord,
    Timeout,
    OutScan,
    ScanCollect,
    ScanAccountCard,

    Glue,
    AutoTighten,
    BlockIncase,
    Pressure,
    LowerBoxGlue,
    DialogImageTighten,
    DialogLeak,
  },
  directives: {
    waves,
    elDragDialog,
  },
  data() {
    return {
      stepMultipleSelection: [], //勾选的数据表值
      stationTakList: [], //工位任务表
      stationtaskTotal: 0, //数据条数
      stationtaskListLoading: true, //加载特效
      dialogStionTaskVisible: false,
      dialogautostationtask: false,
      dialogdpVisible: false,
      dialogbnVisible: false,
      dialogBomVisible: false,
      taskvisible: true,
      stationtaskListQuery: {
        page: 1,
        limit: 20,
        key: undefined,
      },
      taskTemp: {
        id: undefined,
        code: "",
        stepcode: "",
        name: "",
        type: 1,
        stepId: this.stepId,
        productId: this.productId,
        hasPage: false,
        description: "",
        sequence: 1,
        resourceId: undefined,
      },
      tempResource: {
        id: undefined,
        resourceId: "",
        base_StationTaskId: "",
        remark: "",
      },
      stepTemp: {},
      autostationtaskdata: {},
      dialogFormVisible: false, //编辑框
      dialogStatus: "", //编辑框功能(添加/编辑)
      textMap: {
        update: "编辑",
        create: "添加",
      },
      stepRules: {
        code: [
          {
            required: true,
            message: "编号不能为空",
            trigger: "blur",
          },
        ],
        name: [
          {
            required: true,
            message: "名称不能为空",
            trigger: "blur",
          },
        ],
      },
      autoRules: {
        //编辑框输入限制
        programNo: [
          {
            required: true,
            message: "程序号不能为空",
            trigger: "blur",
          },
        ],
        upMesCodePN: [
          {
            required: true,
            message: "上传代码不能为空",
            trigger: "blur",
          },
        ],

      },
      typeoptions: [
        {
          id: 1,
          name: "扫描员工卡",
        },
        {
          id: 2,
          name: "扫码",
        },
        {
          id: 3,
          name: "拧螺丝",
        },

        {
          id: 4,
          name: "扫码输入",
        },
        {
          id: 5,
          name: "用户输入",
        },
        {
          id: 6,
          name: "超时检测",
        },
        {
          id: 7,
          name: "时间记录",
        },
        {
          id: 8,
          name: "称重",
        },
        {
          id: 9,
          name: "补拧",
        },
        {
          id: 10,
          name: "放行",
        },
        {
          id: 11,
          name: "图示拧紧",
        },
        {
          id: 12,
          name: "人工充气",
        },
        {
          id: 31,
          name: "涂胶检测",
        },
        {
          id: 32,
          name: "模组入箱",
        },
        {
          id: 33,
          name: "自动涂胶",
        },
        {
          id: 34,
          name: "自动拧紧",
        },

        {
          id: 35,
          name: "自动加压",
        },

        {
          id: 101,
          name: "下箱体涂胶",
        },
      ],
      stepId: 0,
      productId: 0,
      bomvisible: false,
      angloadvisible: false,
      Dpvisibled: false,
      BNvisible: false,
      timeoutvisiable: false,
      outscanvisiable: false,
      scanCollectVisiable: false,
      scanAccountCardVisible: false,
      recordTimeVisible: false,

      gluevisible: false,
      pressurevisible: false,
      autoTightenVisiable: false,
      blockIncaseVisiable: false,
      lowerBoxGlueVisible: false,
      imageTightenVisible:false,
      leakVisible:false,
      stepname: "",
    };
  },
  created() {
    this.Load();
  },
  mounted() {
    this.Load();
  },
  methods: {
    formatterBoolean(row, column, cellValue) {
      if (cellValue) {
        return "是";
      } else {
        return "否";
      }
    },
    handleDetail() {
      if (this.taskId == 0) {
        this.$notify({
          title: "错误",
          message: "请点击一行数据",
          type: "fail",
          duration: 500,
        });
      }
      this.taskvisible = false;

      switch (this.taskTemp.type) {
        case 1:
          this.scanAccountCardVisible = true;
          break;
        case 2:
          this.bomvisible = true;
          break;
        case 3:
          this.Dpvisibled = true;
          break;
        case 4:
          this.scanCollectVisiable = true;
          break;
        case 5:
          this.outscanvisiable = true;
          break;
        case 6:
          this.timeoutvisiable = true;
          break;
        case 7:
          this.recordTimeVisible = true;
          break;
        case 8:
          this.angloadvisible = true;
          break;
        case 9:
          this.BNvisible = true;
          break;
        case 10:
          this.$notify({
            title: "提示",
            message: "此类型尚未开发界面",
            type: "fail",
            duration: 1000,
          });
          this.taskvisible = true;
          break;
             case 11:
          this.imageTightenVisible = true;
          break
        case 12:
          this.leakVisible = true;
          break;


        case 32:
          this.blockIncaseVisiable = true;
          break;
        case 33:
          this.gluevisible = true;
          break;
        case 34:
          this.autoTightenVisiable = true;
          break;
        case 35:
          this.pressurevisible = true;
          break;
        case 101:
          this.lowerBoxGlueVisible = true;
          break;
        default:
          break;
      }
    },

    //编辑框数值初始值
    resetTaskTemp() {
      this.taskTemp = {
        id: undefined,
        code: "",
        name: "",
        type: 1,
        description: "",
        stepId: this.$parent.stepId,
        productId: this.$parent.productid,
      };
    },
    //工位任务添加

    handleCreateTask() {
      //弹出编辑框
      this.resetTaskTemp(); //数值初始化
      this.taskTemp.stepId = this.$parent.stepId;
      this.taskTemp.productId = this.$parent.productid;

      this.dialogStatus = "create"; //编辑框功能选择（添加）
      this.dialogStionTaskVisible = true; //编辑框显示
      this.$nextTick(() => {
        this.$refs["taskForm"].clearValidate();
      });
    },
    handleUpdateTask() {
      this.dialogStatus = "update"; //编辑框功能选择（更新）
      this.value = this.taskTemp.type;

      this.dialogStionTaskVisible = true; //编辑框显示
      this.$nextTick(() => {
        this.$refs["taskForm"].clearValidate();
      });
    },
    ///选择当前工位任务的值
    handleSelectionStationTaskChange(val) {
      if (val === null) {
        return;
      } else {
        this.taskId = val.id;
        this.taskTemp = val;
      }
    },

    formatterunit(row, column, cellValue) {
      switch (cellValue) {
        case 1:
          return "个";
        case 2:
          return "件";
      }
    },
    setstationTaketype(row, column, cellValue) {
      switch (cellValue) {
        case 1:
          return "扫描员工卡";
        case 2:
          return "扫码";
        case 3:
          return "拧螺丝";
        case 4:
          return "扫码输入";
        case 5:
          return "用户输入";
        case 6:
          return "超时检测";
        case 7:
          return "时间记录";
        case 8:
          return "称重";
        case 9:
          return "补拧";
        case 10:
          return "放行";
        case 11:
          return "图示拧紧";
        case 12:
          return "人工充气";
        case 31:
          return "涂胶检测";
        case 32:
          return "模组入箱";
        case 33:
          return "自动涂胶";
        case 34:
          return "自动拧紧";
        case 35:
          return "自动加压";
        case 101:
          return "下箱体涂胶";
        default:
          return null;
      }
    },

    //工位任务
    createTaskData() {
      this.$refs["taskForm"].validate((valid) => {
        if (valid) {
          this.taskTemp.stepId = this.$parent.stepId;
          this.taskTemp.productId = this.$parent.productid;
          stationtask.add(this.taskTemp).then((response) => {
            this.tempResource.resourceId = this.taskTemp.resourceId;
            this.tempResource.base_StationTaskId = response.result.id;
            this.dialogStionTaskVisible = false; //编辑框关闭
            this.$notify({
              title: "成功",
              message: "创建成功",
              type: "success",
              duration: 2000,
            });
            this.Load();
          });
        }
      });
    },
    updateTaskData() {
      this.$refs["taskForm"].validate((valid) => {
        if (valid) {
          stationtask.update(this.taskTemp).then((response) => {
            this.tempResource.resourceId = this.taskTemp.resourceId;
            this.dialogStionTaskVisible = false; //编辑框关闭
            this.$notify({
              title: "成功",
              message: "更新成功",
              type: "success",
              duration: 2000,
            });
            /// 更新

            this.Load();
          });
        }
      });
    },
    //删除工位任务
    DeleteTask(row) {
      if (row.type == 4 && this.stationTakList.length >= 2) {
        this.$notify({
          title: "警告",
          message: "任务中只有放行任务时方可删除",
          type: "error",
          duration: 2000,
        });
        return;
      }
      this.$confirm("确定要删除吗？")
        .then((_) => {

          //提取复选框的数据的Id
          var selectids = [];
          selectids.push(row.id); //提取复选框的数据的Id
          var params = {
            ids: selectids,
          };
          stationtask.del(params).then(() => {
            this.$notify({
              title: "成功",
              message: "删除成功",
              type: "success",
              duration: 2000,
            });
            this.Load();
          });
        })
        .catch((_) => { });
    },
    Load() {
      if (this.stepId == 0) {
        this.stepId = this.$parent.stepId;
        this.productId = this.$parent.productid;
        this.stepTemp = this.$parent.stepTemp;
        this.stepname = "【" + this.$parent.stepname + "】的配方";
      }
      stationtask
        .LoadTaskByProductStepId({
          stepId: this.$parent.stepId,
          productId: this.$parent.productid,
        })
        .then((response) => {
          this.stationTakList = response.data;

        });
    },
    // GetAutoStationTask() {
    //   stationTaskAuto
    //     .GetByTaskId({ taskid: this.taskTemp.id })
    //     .then((response) => {

    //       this.autostationtaskdata = response.data;
    //       this.dialogautostationtask = true;

    //     });
    // },
    // upautoData() {
    //   this.$refs["autostationForm"].validate((valid) => {
    //     if (valid) {
    //       //自动工位的taskid在手动创建任务是，无法赋值，只能在更新时加入
    //       this.autostationtaskdata.unitid = 1
    //       this.autostationtaskdata.stationtaskid = this.taskTemp.id
    //       stationTaskAuto.Update(this.autostationtaskdata).then((_) => {
    //         this.dialogautostationtask = false;

    //         this.$notify({
    //           title: "成功",
    //           message: "更新成功",
    //           type: "success",
    //           duration: 200,
    //         });
    //         this.Load();
    //       });
    //     }
    //   });
    // },
    //#endregion

    handleDetailUpDown(id, index, upDown, event) {
      if (event.target.nodeName === "I") {
        event.target.parentNode.blur();
      } else {
        event.target.blur();
      }
      if (
        (index == 0 && upDown == -1) ||
        (index == this.stationTakList.length - 1 && upDown == 1)
      ) {
        return;
      }

      var curRow = this.stationTakList[index];
      var upDownRow = this.stationTakList[index + upDown];
      const rowdata = [];
      rowdata.push(
        curRow.id,
        upDownRow.sequence >= this.stationTakList.length
          ? this.stationTakList.length
          : upDownRow.sequence
      );
      rowdata.push(
        upDownRow.id,
        curRow.sequence >= this.stationTakList.length
          ? this.stationTakList.length
          : curRow.sequence
      );

      const data = [];
      for (let index = 0; index < rowdata.length; index++) {
        const element = rowdata[index];
        data.push(element + "," + rowdata[index + 1]);
        index++;
      }
      stationtask.UpTaskOrder(data).then((response) => {
        if (response.result) {
          this.$notify({
            title: "成功",
            message: "调整成功",
            type: "success",
            duration: 2000,
          });

          this.Load();
        }
      });
      // this.stationTakList[index] = upDownRow;
      // this.stationTakList[index + upDown] = curRow;
    },
    handleCurrentChange(val) {
      this.stationtaskListQuery.page = val.page;
      this.stationtaskListQuery.limit = val.limit;
      this.Load(); //页面加载
    },
    back() {
      this.stepId = 0;
      this.productId = 0;
      this.stepname = ""
      this.$parent.steptaskvisible = false;
      this.$parent.indexvisible = true;
      this.$parent.backflowtableSelect();
      this.$parent.resetTemp();
    },
  },
};
</script>
