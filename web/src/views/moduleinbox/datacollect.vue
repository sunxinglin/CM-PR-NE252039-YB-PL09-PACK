<template>
  <div>
    <div class="app-container">
      <el-card shadow="never" class="boby-small" style="height: 100%">
        <div slot="header" class="clearfix">
          <span>入箱数据</span>
        </div>
        <div>
          <el-row :gutter="2">
            <el-col :span="21">
              <el-input prefix-icon="el-icon-search" size="small" style="width: 200px; height: 20px" class="filter-item"
                :placeholder="'产品码'" v-model="query.packCode">
              </el-input>
              <el-input prefix-icon="el-icon-search" size="small" style="width: 200px; height: 20px; margin-left: 10px"
                class="filter-item" :placeholder="'工位'" v-model="query.stationCode">
              </el-input>
              <el-button type="primary" size="small" @click="getlist" style="margin-left: 10px">
                查询
              </el-button>
              <!-- <el-button
                type="primary"
                icon="el-icon-plus"
                size="small"
                @click="ModelExpornt"
              >
                导出
              </el-button> -->
              <el-button type="danger" size="small" @click="upgluedata">
                重传
              </el-button>
            </el-col>
          </el-row>
        </div>
      </el-card>
    </div>
    <div class="app-container">
      <el-table ref="detaillist" :data="detetaillist" v-loading="ListLoading" row-key="id" style="width: 100%" border
        fit stripe highlight-current-row align="left" :height="tablegeight">
        <el-table-column label="Pack码" prop="packCode" align="center" width="300px" />
        <el-table-column label="工位" prop="stationCode" align="center" />
        <el-table-column label="状态" prop="stauts" align="center" :formatter="taskStatus" />
        <el-table-column label="创建日期" prop="createTime" align="center" />
        <el-table-column label="操作" align="center">
          <template slot-scope="scope">
            <el-button size="mini" @click="ViewTaskDetails(scope.row)" type="primary">详情</el-button>
          </template>
        </el-table-column>
      </el-table>
      <div>
        <pagination :total="total" v-show="total > 0" hide-on-single-page :page.sync="query.page"
          :limit.sync="query.limit" @pagination="getlist" />
      </div>
    </div>

    <el-dialog v-el-drag-dialog class="dialog-mini" width="900px" :visible.sync="dialogDetailVisible">
      <div>
        <el-table ref="detailData" :data="detailData" v-loading="ListLoading" row-key="id" style="width: 100%" border
          fit stripe highlight-current-row align="left" :height="tablegeight">
          <el-table-column label="参数名称" prop="parameterName" align="center" width="auto" />
          <el-table-column label="数据类型" prop="moduleInBoxDataType" align="center" :formatter="getDataType"
            width="auto" />
          <el-table-column label="入箱位置" prop="location" align="center" width="auto" />
          <el-table-column label="模组码" prop="moduleCode" align="center" width="auto" />
          <el-table-column label="入箱数据" prop="dataValue" align="center" width="auto" />
          <el-table-column label="上传代码" prop="upMesCode" align="center" width="auto" />
        </el-table>
      </div>
    </el-dialog>
  </div>
</template>
<script>
import waves from "@/directive/waves"; // 水波纹指令
import Sticky from "@/components/Sticky";
import permissionBtn from "@/components/PermissionBtn";
import Pagination from "@/components/Pagination";
import elDragDialog from "@/directive/el-dragDialog";
import * as moduleInBoxAPI from "@/api/moduleinbox";
import { Switch } from "element-ui";
export default {
  components: {
    Sticky,
    permissionBtn,
    Pagination,
  },
  directives: {
    waves,
    elDragDialog,
  },
  mounted() {
    let h = document.documentElement.clientHeight;
    let topH = this.$refs.detaillist.$el.offsetTop;
    this.tablegeight = (h - topH) * 0.81;
    //this.handleClickClose();
  },
  data() {
    return {
      detetaillist: [],
      query: {
        page: 1,
        limit: 20,
        packCode: "",
        stationCode: "",
      },
      tablegeight: null,
      total: 0,
      ListLoading: false,
      pickerOptions1: {},
      pickerOptions0: {},
      dialogDetailVisible: false,
      detailData: [],
    };
  },
  methods: {
    getlist() {
      this.ListLoading = true;
      moduleInBoxAPI.LoadData(this.query).then((response) => {
        this.detetaillist = response.result; //提取数据表
        this.total = response.count; //提取数据表条数
        this.ListLoading = false;
      });
    },
    changeDate() {
      // debugger
      //因为date1和date2格式为 年-月-日， 所以这里先把date1和date2转换为时间戳再进行比较
      let date1 = new Date(this.query.beginTime).getTime();
      let date2 = new Date(this.query.endTime).getTime();

      this.pickerOptions0 = {
        disabledDate: (time) => {
          if (date2 != "") {
            return time.getTime() >= date2;
          }
        },
      };
      this.pickerOptions1 = {
        disabledDate: (time) => {
          return time.getTime() <= date1;
        },
      };
    },
    ModelExpornt() {
      moduleInBoxAPI.modelExpornt(this.query).then((request) => {
        this.$notify({
          title: "提示",
          message: "数据整理完毕,正在下载",
          type: "success",
          duration: 2000,
        });
        var blob = new Blob([request], {
          type: "application/vnd.ms-excel",
        });
        var fileName = "模组入箱数据导出.xlsx";
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

    ViewTaskDetails(val) {
      var incaseId = val.id;
      moduleInBoxAPI.LoadDataDetail({ dataId: incaseId }).then((response) => {
        if (response.code != 200) {
          this.$message({
            message: response.message,
            type: "error",
          });
          return;
        }
        this.detailData = response.result;
        this.dialogDetailVisible = true;
        console.log(this.detailData);
      });
    },
    upgluedata() {
      if (this.query.packCode == null || this.query.packCode == "") {
        this.$notify({
          title: "错误",
          message: "未提供PACK码",
          type: "error",
          duration: 2000,
        });
        return;
      }
      if (this.query.stationCode == null || this.query.stationCode == "") {
        this.$notify({
          title: "错误",
          message: "未提供工位",
          type: "error",
          duration: 2000,
        });
        return;
      }
      (this.query.isNeedChangeStatus = true),
        moduleInBoxAPI.upCatlAgain({ packCode: this.query.packCode, stationCode: this.query.stationCode, isNeedChangeStatus: true }).then((response) => {
          if (response.code != 200) {
            this.$notify({
              title: "错误",
              message: response.message,
              type: "error",
              duration: 2000,
            });
            return;
          }
          if (response.result.code != 0) {
            this.$notify({
              title: "错误",
              message: response.result.message,
              type: "error",
              duration: 2000,
            });
            return;
          }

          this.$notify({
            title: "提示",
            message: "上传成功",
            type: "success",
            duration: 2000,
          });
        });
    },
    getDataType(row, column, cellValue) {
      switch (row.moduleInBoxDataType) {
        case 1:
          return "模组码";
        case 2:
          return "模组长度";
        case 3:
          return "保压时间";
        case 4:
          return "下压距离";
        case 5:
          return "下压压力";
        case 6:
          return "左侧压力";
        case 7:
          return "右侧压力";
        case 8:
          return "入箱完成时间";
        case 9:
          return "模组入箱时长";
      }
    },
    taskStatus(row, column, cellValue) {
      switch (row.stauts) {
        case 0:
          return "未开始";
        case 1:
          return "进行中";
        case 2:
          return "已完成";
      }
    },
  },
};
</script>