<template>
  <div>
    <div class="app-container">
      <el-card shadow="never" class="boby-small" style="height: 100%">
        <div slot="header" class="clearfix">
          <span>自动拧紧详情数据</span>
        </div>
        <div>
          <el-row :gutter="2">
            <el-col :span="21">
              <el-input prefix-icon="el-icon-search" size="small" style="width: 200px; height: 20px" class="filter-item"
                :placeholder="'pack码'" v-model="query.packCode">
              </el-input>

              <el-date-picker
                clearable
                filterable
                size="small"
                class="filter-item"
                placeholder="开始日期"
                type="date"
                :editable="false"
                v-model="query.beginTime"
                value-format="yyyy-MM-dd"
                @change="changeDate"
                :picker-options="pickerOptions0"
              >
              </el-date-picker>

              <el-date-picker
                clearable
                filterable
                size="small"
                class="filter-item"
                placeholder="结束日期"
                type="date"
                :editable="false"
                v-model="query.endTime"
                @change="changeDate"
                :picker-options="pickerOptions1"
                value-format="yyyy-MM-dd"
              >
              </el-date-picker>

              <el-button
                icon="el-icon-search"
                size="small"
                @click="getlist"
                style="margin-left: 10px"
              >
                查询
              </el-button>
              <el-button
                type="primary"
                icon="el-icon-plus"
                size="small"
                @click="ModelExpornt"
              >
                导出
              </el-button>
            </el-col>
          </el-row>
        </div>
      </el-card>
    </div>
    <div class="app-container">
      <el-table ref="detaillist" :data="dataList" v-loading="ListLoading" row-key="id" style="width: 100%" border fit
        stripe highlight-current-row align="left" :height="tablegeight">
        <el-table-column label="Pack编码" prop="packPN" align="center" width="300px" />
        <el-table-column label="扭力上传代码" prop="uploadCode" align="center" />
        <el-table-column label="角度上传代码" prop="uploadCode_JD" align="center" />
        <el-table-column label="自动拧紧类型" prop="boltType" align="center" :formatter="setTightenReworktype" />
        <el-table-column label="操作" align="center">
          <template slot-scope="scope">
            <el-button size="mini" @click="ViewTaskDetails(scope.row)" type="success">查看详情</el-button>
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
          <el-table-column label="序号" prop="orderNo" align="center" width="auto" />
          <el-table-column label="拧紧结果" prop="resultIsOK" align="center" width="auto">
            <template slot-scope="scope">
              <span>
                {{ scope.row.resultIsOK ? "OK" : "NG" }}
              </span>
            </template>
          </el-table-column>
          <el-table-column label="程序号" prop="programNo" align="center" width="auto" />
          <el-table-column label="扭力" prop="finalTorque" align="center" width="auto" />
          <el-table-column label="角度" prop="finalAngle" align="center" width="auto" />
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
import * as blotgundetail from "@/api/blotgundetail";
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
    this.changeDate();
  },
  data() {
    return {
      dataList: [],
      detailData: [],
      query: {
        page: 1,
        limit: 20,
        packCode: "",
      },
      tablegeight: null,
      total: 0,
      ListLoading: false,
      pickerOptions1: {},
      pickerOptions0: {},
      dialogDetailVisible: false,
    };
  },
  methods: {
    getlist() {
      this.ListLoading = true;
      blotgundetail.getAutoBlotList(this.query).then((response) => {
        this.dataList = response.result; //提取数据表
        this.total = response.count; //提取数据表条数
        console.log(this.total);
        this.ListLoading = false;
      });
    },
    handleFilter() {
      this.query.page = 1;
      this.getlist();
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
    setTightenReworktype(row, column, cellValue) {
      switch (cellValue) {
        case "Module":
          return "模组拧紧";
        case "Lid":
          return "上盖拧紧";
        default:
          return null;
      }
    },
    ViewTaskDetails(val) {
      var incaseId = val.id;
      blotgundetail
        .getAutoBlotDetailList({ dataId: incaseId })
        .then((response) => {
          if (response.code != 200) {
            this.$message({
              message: response.message,
              type: "error",
            });
            return;
          }
          this.detailData = response.result;
          this.dialogDetailVisible = true;
        });
    },
    ModelExpornt() {
      blotgundetail.automodelExpornt(this.query).then((response) => {
        this.$notify({
          title: "提示",
          message: "数据整理完毕,正在下载",
          type: "success",
          duration: 2000,
        });
        window.location.href = `/api/StationTask_BlotGunDetail/DownloadExcel/${response.result}`
      });
    },
  },
};
</script>