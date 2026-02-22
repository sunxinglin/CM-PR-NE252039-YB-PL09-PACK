<template>
  <div>
    <div class="app-container">
      <el-col :span="24">
        <el-card shadow="never" class="boby-small" style="height: 100%">
          <div slot="header" class="clearfix">
            <span>扫码任务详情</span>
          </div>
          <div style="margin-bottom: 10px">
            <el-row :gutter="4">
              <el-col :span="20">
                <el-button type="warning" icon="el-icon-back" size="mini" @click="back">返回</el-button>
                <el-button type="primary" icon="el-icon-plus" size="mini" @click="handleDetailAdd">添加</el-button>
                <el-button type="primary" icon="el-icon-edit" size="mini" @click="handleDetailEdit">编辑</el-button>
                <el-button type="primary" icon="el-icon-delete" size="mini" @click="handleDetailDelete">删除</el-button>
              </el-col>
            </el-row>
          </div>
          <div>
            <el-table :data="bomData" ref="detailTable" row-key="id" @row-click="detailrowclick"
              @current-change="handleSelectionBomChange" border fit stripe highlight-current-row align="left">
              <!-- <el-table-column
                property="goodsCode"
                label="编码"
                width="130"
              ></el-table-column> -->
              <el-table-column property="goodsName" label="名称" align="center" min-width="50"></el-table-column>
              <el-table-column property="useNum" label="数量" min-width="60" align="center"></el-table-column>
              <el-table-column property="tracingType" label="MES追溯类型" min-width="100" align="center"
                :formatter="formattertracingType">
              </el-table-column>

              <el-table-column property="goodsPN" label="物料PN" align="center" min-width="100"></el-table-column>
              <el-table-column property="goodsPNRex" label="内部条码规则" min-width="100" align="center"></el-table-column>
              <el-table-column property="outerGoodsPNRex" label="外部条码规则" min-width="130" align="center"></el-table-column>
              <!-- <el-table-column
                property="hasOuterParam"
                label="外部输入"
                min-width="100"
                align="center"
                :formatter="formatterBoolean"
              ></el-table-column>
              <el-table-column
                property="outerParam1"
                label="外部输入1"
                align="center"
                min-width="100"
                :formatter="switchInCometype"
              ></el-table-column>
              <el-table-column
                property="outerParam2"
                label="外部输入2"
                min-width="100"
                align="center"
                :formatter="switchInCometype"
              ></el-table-column>
              <el-table-column
                property="outerParam3"
                label="外部输入3"
                min-width="100"
                align="center"
                :formatter="switchInCometype"
              ></el-table-column> -->
              <el-table-column property="remark" label="备注"></el-table-column>
            </el-table>
          </div>
        </el-card>
      </el-col>
    </div>
    <!-- BOM弹框1-->
    <!-- BOM弹框2-->
    <el-dialog v-el-drag-dialog class="dialog-mini" width="600px" :title="textMap[dialogStatus]"
      :visible.sync="dialogBomVisible">
      <div>
        <el-form :rules="bomRules" ref="bomForm" :model="bomTemp" label-position="right" label-width="100px">

          <el-form-item size="small" :label="'名称'" prop="goodsName">
            <el-input v-model="bomTemp.goodsName"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'使用数量'">
            <el-input-number v-model="bomTemp.useNum" :min="1"></el-input-number>
          </el-form-item>
          <el-form-item size="small" :label="'MES追溯类型'" prop="tracingType">
            <el-select size="medium" v-model="bomTemp.tracingType" placeholder="请选择" @change="tracingTypeSelectChange">
              <el-option v-for="item in tracingTypes" :key="item.key" :label="item.display" :value="item.key">
              </el-option>
            </el-select>
            <!-- <el-switch v-show="bomTemp.tracingType == 1"
  v-model="bomTemp.needCollectMESData"
  active-text="合并上传MES数据">
</el-switch> -->

          </el-form-item>
          <el-form-item size="small" :label="'物料PN'" prop="goodsPN">
            <el-input v-model="bomTemp.goodsPN"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="goodRexlable" prop="goodsPNRex">
            <el-input v-model="bomTemp.goodsPNRex"></el-input>
          </el-form-item>
          <el-form-item v-show="bomTemp.tracingType == 1" size="small" :label="'外部条码规则'" prop="outerGoodsPNRex">
            <el-input v-model="bomTemp.outerGoodsPNRex"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'备注'">
            <el-input v-model="bomTemp.remark"></el-input>
          </el-form-item>
          <!-- <el-form-item size="small" :label="'是否外部输入'" label-width="150px">
            <el-switch v-model="bomTemp.hasOuterParam"></el-switch>
          </el-form-item>
          <el-form-item v-show="bomTemp.hasOuterParam" size="small" :label="'外部输入1'">
            <el-radio-group v-model="bomTemp.outerParam1">
              <el-radio :label="1">电压</el-radio>
              <el-radio :label="2">电阻</el-radio>
              <el-radio :label="3">电流</el-radio>
              <el-radio :label="4">压力</el-radio>
              <el-radio :label="0">无输入</el-radio>
            </el-radio-group>
            <el-form-item v-show="bomTemp.outerParam1 != 0" size="small" :label="'上传代码'">
              <el-input v-model="bomTemp.upMesCode1"></el-input>
            </el-form-item>
            <el-form-item v-show="bomTemp.outerParam1 != 0" size="small" :label="'最小值'">
              <el-input size="small" v-model="bomTemp.minOuterParam1"></el-input>
            </el-form-item>
            <el-form-item v-show="bomTemp.outerParam1 != 0" size="small" :label="'最大值'">
              <el-input size="small" v-model="bomTemp.maxOuterParam1"></el-input>
            </el-form-item>
          </el-form-item>
       
          <el-form-item v-show="bomTemp.hasOuterParam" size="small" :label="'外部输入2'">
            <el-radio-group v-model="bomTemp.outerParam2">
              <el-radio :label="1">电压</el-radio>
              <el-radio :label="2">电阻</el-radio>
              <el-radio :label="3">电流</el-radio>
              <el-radio :label="4">压力</el-radio>
              <el-radio :label="0">无输入</el-radio>
            </el-radio-group>
            <el-form-item v-show="bomTemp.outerParam2 != 0" size="small" :label="'上传代码'">
              <el-input v-model="bomTemp.upMesCode2"></el-input>
            </el-form-item>
            <el-form-item v-show="bomTemp.outerParam2 != 0" size="small" :label="'最小值'">
              <el-input v-model="bomTemp.minOuterParam2"></el-input>
            </el-form-item>
            <el-form-item v-show="bomTemp.outerParam2 != 0" size="small" :label="'最大值'">
              <el-input v-model="bomTemp.maxOuterParam2"></el-input>
            </el-form-item>
          </el-form-item>
          <el-form-item v-show="bomTemp.hasOuterParam" size="small" :label="'外部输入3'">
            <el-radio-group v-model="bomTemp.outerParam3">
              <el-radio :label="1">电压</el-radio>
              <el-radio :label="2">电阻</el-radio>
              <el-radio :label="3">电流</el-radio>
              <el-radio :label="4">压力</el-radio>
              <el-radio :label="0">无输入</el-radio>
            </el-radio-group>
            <el-form-item v-show="bomTemp.outerParam3 != 0" size="small" :label="'上传代码'">
              <el-input v-model="bomTemp.upMesCode3"></el-input>
            </el-form-item>

            <el-form-item v-show="bomTemp.outerParam3 != 0" size="small" :label="'最小值'">
              <el-input v-model="bomTemp.minOuterParam3"></el-input>
            </el-form-item>
            <el-form-item v-show="bomTemp.outerParam3 != 0" size="small" :label="'最大值'">
              <el-input v-model="bomTemp.maxOuterParam3"></el-input>
            </el-form-item>
          </el-form-item> -->
        </el-form>
      </div>
      <div slot="footer">
        <el-button size="mini" @click="dialogBomVisible = false">取消</el-button>
        <el-button size="mini" v-if="dialogStatus == 'create'" type="primary" @click="createBomData">确认</el-button>
        <el-button size="mini" v-else type="primary" @click="updateBomData">确认</el-button>
      </div>
    </el-dialog>
    <!-- BOM弹框2-->
  </div>
</template>

<script>
import * as stationtaskbom from "@/api/stationTaskBom";
import elDragDialog from "@/directive/el-dragDialog";
import waves from "@/directive/waves"; // 水波纹指令
export default {
  directives: {
    waves,
    elDragDialog,
  },
  data() {
    return {
      dialogStatus: "", //编辑框功能(添加/编辑)
      bomRowid: 0,
      textMap: {
        update: "编辑",
        create: "添加",
        detail: "任务详情",
      },
      bomRules: {
        goodsName: [
          {
            required: true,
            message: "名称不能为空",
            trigger: "blur",
          },
        ],
        goodsPN: [
          {
            required: true,
            message: "物料PN不能为空",
            trigger: "blur",
          },
        ],
        // goodsPNRex: [
        //   {
        //     required: true,
        //     message: "条码规则不能为空",
        //     trigger: "blur",
        //   },
        // ],
        // outerGoodsPNRex: [
        //   {
        //     required: true,
        //     message: "外部条码规则不能为空",
        //     trigger: "blur",
        //   },
        // ],
        // useNum: [
        //   {required: true,  min: 1, message: "最低不能低于1", trigger: "blur" },
        // ],
      },
      bomData: [],
      bomTemp: {
        id: undefined,

        goodsName: "",
        useNum: 1,
        unitId: 1,
        stationTaskId: 1,
        hasOuterParam: false,
        needCollectMESData: false,
        outerParam1: 0,
        outerParam2: 0,
        outerParam3: 0,
        minOuterParam1: 0,
        minOuterParam2: 0,
        minOuterParam3: 0,
        maxOuterParam1: 0,
        maxOuterParam2: 0,
        maxOuterParam3: 0,
        remark: "",
        goodsPN: "",
        goodsPNRex: "",
        outerGoodsPNRex: "",
        tracingType: 0,
      },
      tracingTypes: [
        {
          display: "精追",
          key: 1,
        },
        {
          display: "批追",
          key: 2,
        },
        {
          display: "扫库存",
          key: 3,
        },
      ],
      taskId: "",
      dialogBomVisible: false,
      goodRexlable: "批次条码规则",
    };
  },
  mounted() {
    this.load();
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
    formattertracingType(row, column, cellValue) {
      switch (cellValue) {
        case 1:
          return "精追";
        case 2:
          return "批追";
        case 3:
          return "扫库存";
        default:
          break;
      }
    },
    handleSelectionBomChange(val) {
      if (val === null) {
        return;
      } else {
        this.bomTemp.stationTaskId = val.stationTaskId;

        this.bomTemp.goodsName = val.goodsName;
        this.bomTemp.useNum = val.useNum;
        this.bomTemp.goodsPN = val.goodsPN;
        this.bomTemp.goodsPNRex = val.goodsPNRex;
        this.bomTemp.outerGoodsPNRex = val.outerGoodsPNRex;
        this.bomTemp.hasOuterParam = val.hasOuterParam;
        this.bomTemp.needCollectMESData = val.needCollectMESData;

        this.bomTemp.outerParam1 = val.outerParam1;
        this.bomTemp.minOuterParam1 = val.minOuterParam1;
        this.bomTemp.maxOuterParam1 = val.maxOuterParam1;

        this.bomTemp.outerParam2 = val.outerParam2;
        this.bomTemp.minOuterParam2 = val.minOuterParam2;
        this.bomTemp.maxOuterParam2 = val.maxOuterParam2;

        this.bomTemp.outerParam3 = val.outerParam3;
        this.bomTemp.minOuterParam3 = val.minOuterParam3;
        this.bomTemp.maxOuterParam3 = val.maxOuterParam3;

        this.bomTemp.remark = val.remark;
        this.bomTemp.id = val.id;
        this.bomTemp.tracingType = val.tracingType;
      }
    },
    detailrowclick(row) {
      ///获取当前的行号
      this.bomRowid = row.id;
    },

    resetdetial() {
      this.bomTemp = {
        id: undefined,

        goodsName: "",
        useNum: 1,
        unitId: 1,
        stationTaskId: 1,
        hasOuterParam: false,
        needCollectMESData: false,
        outerParam1: 0,
        minOuterParam1: 0,
        maxOuterParam1: 0,
        outerParam2: 0,
        minOuterParam2: 0,
        maxOuterParam2: 0,
        outerParam3: 0,
        minOuterParam3: 0,
        maxOuterParam3: 0,
        remark: "",
        goodsPN: "",
        goodsPNRex: "",
        outerGoodsPNRex: "",
        tracingType: 1,
      };
    },

    tracingTypeSelectChange(val) {
      switch (val) {
        case 1:
        case 2:
          this.goodRexlable = "批次条码规则";

          return;
        case 3:
          this.goodRexlable = "库存条码规则";

          return;
        default:
          break;
      }
    },
    ///详情添加
    handleDetailAdd() {
      console.log(this.bomData);
      if (this.bomData.length >= 1) {
        this.$message({
          message: "只可添加一个任务",
          type: "error",
        });
        return;
      }
      this.dialogStatus = "create"; //编辑框功能选择（添加）
      this.dialogBomVisible = true; //编辑框显示
      this.goodRexlable = "批次条码规则";
      this.$nextTick(() => {
        this.$refs["bomForm"].clearValidate();
      });
      this.resetdetial();
    },
    ///详情编辑
    handleDetailEdit() {


      if (this.bomTemp.id != undefined) {
        this.dialogStatus = "update"; //编辑框功能选择（添加）
        this.tracingTypeSelectChange(this.bomTemp.tracingType);
        this.dialogBomVisible = true; //编辑框显示

        this.$nextTick(() => {
          this.$refs["bomForm"].clearValidate();
        });
      } else {
        this.$message({
          message: "请选择一个想要修改的数据",
          type: "error",
        });
      }
    },
    ///详情删除
    handleDetailDelete() {
      if (this.bomTemp.id === undefined) {
        this.$message({
          message: "请选择一个想要删除的数据",
          type: "error",
        });
        return;
      }
      this.$confirm("确定要删除吗？")
        .then(() => {
          //提取复选框的数据的Id
          var selectids = [];
          selectids.push(this.bomTemp.id); //提取复选框的数据的Id
          var params = {
            ids: selectids,
          };
          stationtaskbom.del(params).then(() => {
            this.$notify({
              title: "成功",
              message: "删除成功",
              type: "success",
              duration: 2000,
            });
            this.$nextTick(() => { });
            this.load(); //页面加载
          });
        })
        .catch(() => { });
    },

    createBomData() {
      this.$refs["bomForm"].validate((valid) => {
        if (valid) {

          this.bomTemp.stationTaskId = this.taskId;

          stationtaskbom.add(this.bomTemp).then(() => {
            this.dialogBomVisible = false; //编辑框关闭
            this.$notify({
              title: "成功",
              message: "创建成功",
              type: "success",
              duration: 2000,
            });
            this.load();
          });
        }
      });
    },

    updateBomData() {
      this.$refs["bomForm"].validate((valid) => {
        if (valid) {

          // this.bomTemp.stationTaskId = this.taskTemp.id;
          stationtaskbom.update(this.bomTemp).then(() => {
            this.dialogBomVisible = false; //编辑框关闭
            this.$notify({
              title: "成功",
              message: "修改成功",
              type: "success",
              duration: 2000,
            });
            this.load();
          });
        }
      });
    },

    switchInCometype(row, column, cellValue) {
      switch (cellValue) {
        case 0:
          return "无输入";
        case 1:
          return "电压";
        case 2:
          return "电阻";
        case 3:
          return "电流";
        case 4:
          return "压力";
      }
    },
    load() {
      if (this.taskId == 0) {
        this.taskId = this.$parent.taskId;
      }

      stationtaskbom.Load({ taskid: this.taskId }).then((response) => {
        console.log(response.data);
        this.bomData = response.data;
      });
    },
    back() {
      console.log(this.taskId);
      this.taskId = 0;
      this.$parent.bomvisible = false;
      this.$parent.taskvisible = true;
    },
  },
  // props: {
  //     taskid: {
  //       type: String,
  //       default: "",
  //     },
  //   },
};
</script>

