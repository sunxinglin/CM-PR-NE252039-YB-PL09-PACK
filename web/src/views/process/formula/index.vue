<template>
  <div class="flex-column">
    <div v-if="indexvisible" style="height: 100%" v-loading.fullscreen.lock="fullscreenloading">

      <div class="filter-container">
        <el-card shadow="never" class="boby-small" style="height: 100%">
          <div slot="header" class="clearfix">
            <span>工序表</span>
          </div>
          <div>
            <el-row :gutter="2">
              <el-col :span="21">
                <el-button type="primary" icon="el-icon-menu" size="small" @click="handleTaskDetail">
                  配方详情</el-button>
                <el-button type="primary" size="small" icon="el-icon-download" @click="importfilebtn">
                  导入配方 </el-button>
                <el-button type="primary" icon="el-icon-upload2" size="small" @click="ModelExpornt">
                  导出配方</el-button>
              </el-col>
              <el-col :span="3">
                <el-input @keyup.enter.native="handleFilter" prefix-icon="el-icon-search" size="small"
                  style="width: 200px" class="filter-item" :placeholder="'关键字'" v-model="stepListQuery.key"></el-input>
              </el-col>
            </el-row>
          </div>
        </el-card>
      </div>

      <div class="app-container fh">
        <el-row :gutter="20">
          <el-col :span="6">
            <el-table ref="productTable" :data="productList" v-loading="productListLoading" row-key="id"
              style="width: 100%" :height="tableHeight" @row-click="productRowClick"
              @selection-change="productSelectionChange" border fit stripe highlight-current-row align="left">
              <el-table-column type="selection" min-width="20px" align="center"></el-table-column>
              <el-table-column prop="code" label="PN号" min-width="20px" sortable align="center"></el-table-column>
              <el-table-column prop="name" label="名称" min-width="20px" sortable align="center"></el-table-column>
              <!-- <el-table-column prop="specification" label="产品描述" min-width="100px" sortable align="center">
            </el-table-column> -->
            </el-table>
            <pagination :total="productTotal" v-show="productTotal > 0" :page.sync="productListQuery.page"
              :limit.sync="productListQuery.limit" @pagination="producthandleCurrentChange" />
          </el-col>
          <el-col :span="18">
            <el-table ref="stepTable" :data="stepList" v-loading="stepListLoading" row-key="id" style="width: 100%"
              :height="tableHeight2" @row-click="stepRowClick" @selection-change="handleSelectionChange" border fit
              stripe highlight-current-row align="left">
              <el-table-column type="selection" min-width="20px" align="center"></el-table-column>
              <el-table-column prop="code" label="编码" min-width="20px" sortable align="center"></el-table-column>
              <el-table-column prop="name" label="名称" min-width="20px" sortable align="center"></el-table-column>
              <el-table-column prop="steptype" label="类型" min-width="20px" :formatter="changeStatus"
                align="center"></el-table-column>
              <!-- 		<el-table-column prop="description" label="描述" min-width="20px" sortable align="center"></el-table-column> -->
            </el-table>

            <pagination :total="stepTotal" v-show="stepTotal > 0" :page.sync="stepListQuery.page"
              :limit.sync="stepListQuery.limit" @pagination="handleCurrentChange" />
          </el-col>
        </el-row>

      </div>

      <!-- 导入弹框-->
      <el-dialog :visible.sync="importfilevisible">
        <el-upload ref="upload" action="string" :auto-upload="false" :on-change="elInFile" :limit="1" :multiple="false"
          drag accept=".xlsx,.xls">
          <i class="el-icon-upload"></i>
          <div class="el-upload__text">将文件拖到此处，或<em>点击选择</em></div>
        </el-upload>
        <el-progress :text-inside="true" :stroke-width="26" :percentage="progressvalue" />
        <div slot="footer">
          <el-button type="primary" size="small" @click="importfilevisible = false">取消</el-button>

          <el-button type="primary" size="small" @click="importfile">确认</el-button>
        </div>
      </el-dialog>

    </div>
    <Stationtask ref="stationtask" v-if="steptaskvisible" style="height: 100%;" />

  </div>
</template>

<script>
import * as steps from "@/api/step";
import Stationtask from "./stationtask.vue";

import * as products from "@/api/product";
import * as flow from "@/api/flow";
import * as axios from "axios";
import waves from "@/directive/waves"; // 水波纹指令
import Sticky from "@/components/Sticky";
import permissionBtn from "@/components/PermissionBtn";
import Pagination from "@/components/Pagination";
import elDragDialog from "@/directive/el-dragDialog";
export default {
  name: "step",

  components: {
    Sticky,
    permissionBtn,
    Pagination,
    Stationtask,
  },
  directives: {
    waves,
    elDragDialog,
  },
  data() {
    return {
      stepname: "",
      indexvisible: true,
      steptaskvisible: false,
      selectedProductRowId: null,
      selectedStepRowId: null,
      stepMultipleSelection: [], //勾选的数据表值
      StationTaskExporn: {
        StepIds: [],
        ProductId: 0,
      },
      codeinputdis: true,
      stepList: [], //数据表
      productList: [], //数据表
      productListLoading: true, //加载特效
      productListQuery: {
        //查询条件
        page: 1,
        limit: 20,
        key: undefined,
      },
      stepId: 0,
      stepTotal: 0, //数据条数
      productTotal: 0, //数据条数
      productid: 0,
      stepListLoading: false, //加载特效
      tableHeight: null,
      tableHeight2:null,
      progressvisible: "hidden",
      progressvalue: 0,
      importfilevisible: false,
      fullscreenloading: false,

      stepListQuery: {
        //查询条件
        page: 1,
        limit: 20,
        productid: 0,
        key: undefined,
      },
      stepTemp: {
        //模块临时值
        id: undefined,
        code: "",
        name: "",
        flowId: "",
        description: "",
        steptype: 0
      },

      options: [
        {
          type: 1,
          label: "自动站",
        },
        {
          type: 2,
          label: "线内人工站",
        },
        {
          type: 3,
          label: "线外人工站",
        },
        {
          type: 4,
          label: "可跳过人工站",
        },
      ],
      dialogFormVisible: false, //编辑框
      dialogStatus: "", //编辑框功能(添加/编辑)
      textMap: {
        update: "编辑",
        create: "添加",
      },
      stepRules: {
        //编辑框输入限制
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
      excelfiles: [],

    };
  },
  mounted() {
    let h = document.documentElement.clientHeight; // 可见区域高度
    let topH = this.$refs.productTable.$el.offsetTop; //表格距浏览器顶部距离
    let tableHeight = (h - topH) * 0.74; //表格应该有的高度   乘以多少可自定义
    this.tableHeight = tableHeight;
    this.tableHeight2 = tableHeight+50;
    this.productLoad();
  },
  methods: {
    changeStatus(row, column, cellValue) {
      switch (cellValue) {
        case 1:
          return "自动站";
        case 2:
          return "线内人工站";
        case 3:
          return "线外人工站";
        case 4:
          return "可跳过人工站";
      }
    },

    importfilebtn() {
      this.importfilevisible = true
      this.progressvalue = 0
    },

    //勾选框
    handleSelectionChange(val) {
      console.log("点击行############");
      console.log(this.productid);
      console.log(val);
      this.resetTemp();

      if (val.length == 1) {
        this.stepTemp = val[0];
        this.stepId = val[0].id;
        this.id = val[0].id;
        this.stationtablerowselect = val[0]
        this.stepname = val[0].name
      }

      console.log(this.stepTemp);

      this.stepMultipleSelection = val;
      if (val.length === 1) {
        this.selectedStepRowId = val[0].id;
      } else if (val.length === 0) {
        this.selectedStepRowId = null;
      } else {
        this.selectedStepRowId = null;
      }
    },
    producthandleSelectionChange(val) {

      this.productid = val.id
      this.stepListQuery.productid = this.productid
      this.$refs.productTable.clearSelection();
      this.$refs.productTable.toggleRowSelection(val);
      this.stepList = []
      this.stepLoad()
    },
    productSelectionChange(val) {
      if (val.length === 1) {
        const row = val[0];
        this.selectedProductRowId = row.id;
        this.productid = row.id;
        this.stepListQuery.productid = row.id;

        this.selectedStepRowId = null;
        this.stepMultipleSelection = [];
        this.resetTemp();
        if (this.$refs.stepTable) {
          this.$refs.stepTable.clearSelection();
        }

        this.stepList = [];
        this.stepLoad();
        return;
      }

      if (val.length === 0) {
        this.selectedProductRowId = null;
        this.productid = 0;
        this.stepListQuery.productid = 0;

        this.selectedStepRowId = null;
        this.stepMultipleSelection = [];
        this.resetTemp();
        if (this.$refs.stepTable) {
          this.$refs.stepTable.clearSelection();
        }

        this.stepList = [];
        this.stepTotal = 0;
        return;
      }

      const lastRow = val[val.length - 1];
      const table = this.$refs.productTable;
      if (!table) return;
      table.clearSelection();
      table.toggleRowSelection(lastRow, true);
    },
    productRowClick(row, column) {
      if (column && column.type === "selection") return;

      const table = this.$refs.productTable;
      if (!table) return;

      const isSameRow = this.selectedProductRowId === row.id;
      table.clearSelection();

      if (isSameRow) {
        this.selectedProductRowId = null;
        this.productid = 0;
        this.stepListQuery.productid = 0;

        this.selectedStepRowId = null;
        this.stepMultipleSelection = [];
        this.resetTemp();
        if (this.$refs.stepTable) {
          this.$refs.stepTable.clearSelection();
        }

        this.stepList = [];
        this.stepTotal = 0;
        return;
      }

      table.toggleRowSelection(row, true);
      this.selectedProductRowId = row.id;
    },
    stepRowClick(row, column) {
      if (column && column.type === "selection") return;

      const table = this.$refs.stepTable;
      if (!table) return;

      const isSameRow = this.selectedStepRowId === row.id;
      table.clearSelection();
      if (isSameRow) {
        this.selectedStepRowId = null;
        return;
      }
      table.toggleRowSelection(row, true);
      this.selectedStepRowId = row.id;
    },
    //关键字搜索
    handleFilter() {
      this.stepLoad();
    },
    //分页
    handleCurrentChange(val) {
      this.stepListQuery.page = val.page;
      this.stepListQuery.limit = val.limit;
      this.stepLoad(); //页面加载
    },
    producthandleCurrentChange(val) {
      this.productListQuery.page = val.page;
      this.productListQuery.limit = val.limit;
      this.productid = 0
      this.productLoad(); //页面加载
    },
    //列表加载
    stepLoad() {
      this.stepListLoading = true;
      steps.GetStepsByProductId(this.stepListQuery).then((response) => {
        this.stepList = response.result; //提取数据表
        this.stepTotal = response.count; //提取数据表条数
        console.log(this.productid);
        this.stepListLoading = false;
      });
    },
    //产品列表加载
    productLoad() {
      this.productListLoading = true;
      products.load(this.productListQuery).then((response) => {
        this.productList = response.data; //提取数据表
        this.productTotal = response.count; //提取数据表条数
        this.productListLoading = false;
      });
    },
    //编辑框数值初始值
    resetTemp() {
      this.stepTemp = {
        id: undefined,
        code: "",
        name: "",
        flowId: "",
        description: "",
      };
      this.codeinputdis = false
    },
    elInFile(f, fs) {
      this.excelfiles = fs;
    },
    importfile() {
      var form = new FormData();
      if (this.excelfiles.length <= 0) {
        this.$message("请选择文件后重试!");
        return;
      }
      this.$refs.upload.clearFiles();
      form.append("file", this.excelfiles[0].raw);
      this.excelfiles = [];

      this.progressvisible = "visible";
      this.progressvalue = 50
      steps
        .imporntStationFile(form)
        .then((reponse) => {
          this.$notify({
            title: "成功",
            message: "导入成功",
            type: "success",
            duration: 2000,
          });
          this.importfilevisible = false;
          this.progressvisible = "hidden";
          this.progressvalue = 0
          this.stationLoad();
        });
    },
    ModelExpornt() {
      if (this.stepMultipleSelection.length <= 0) {
        this.$message("请选择工位后重试!");
        return;
      }
      // this.fullscreenloading = true;
      this.stepMultipleSelection.map((o) => o.id)
      this.StationTaskExporn.StepIds = this.stepMultipleSelection.map((o) => o.id)
      this.StationTaskExporn.ProductId = this.productid
      steps
        .ModelExpornt(this.StationTaskExporn)
        .then((request) => {
          // this.fullscreenloading = false;
          this.$notify({
            title: "提示",
            message: "数据整理完毕,正在下载",
            type: "success",
            duration: 2000,
          });
          var blob = new Blob([request], {
            type: "application/vnd.ms-excel",
          });
          var fileName = "工位配方导入导出.xlsx";
          if (window.navigator.msSaveOrOpenBlob) {
            navigator.msSaveBlob(blob, fileName);
          } else {
            var link = document.createElement("a");

            link.href = window.URL.createObjectURL(blob);
            link.download = fileName;
            link.click();
            window.URL.revokeObjectURL(link.href);
          }
        });
    },


    handleStationDetail() {
      if (
        this.stepTemp.id == undefined ||
        this.stepMultipleSelection.length == 0
      ) {
        this.$message({
          message: "请选择需要查看的数据",
          type: "error",
          duration: 5 * 1000,
        });
        return;
      }
      console.log(this.stepMultipleSelection);
      if (this.stepMultipleSelection.length > 1) {
        this.$message({
          message: "请选择单条数据!",
          type: "error",
          duration: 5 * 1000,
        });
        return;
      }



      const vm = this;
      console.log(this.stepTemp.id);
      vm.$router.push({
        path: '/Process/station/Index',
        query: {
          stepId: this.stepTemp.id
        }
      });
    },

    handleTaskDetail() {
      if (
        this.stepTemp.id == undefined ||
        this.stepMultipleSelection.length == 0
      ) {
        this.$message({
          message: "请选择需要查看的数据",
          type: "error",
          duration: 5 * 1000,
        });
        return;
      }
      if (this.stepMultipleSelection.length > 1) {
        this.$message({
          message: "请选择单条数据!",
          type: "error",
          duration: 5 * 1000,
        });
        return;
      }
      this.steptaskvisible = true;
      this.indexvisible = false;
      this.$refs.stationtask.Load();
    },

    checkstep() {
      flow.GetMapByStepId({ "stepid": this.stepTemp.id }).then((response) => {
        this.codeinputdis = response.result
      })
    }
  },
};
</script>
