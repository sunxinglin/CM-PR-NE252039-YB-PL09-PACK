<template>
  <div class="app-container">
    <el-col :span="24">
      <el-card shadow="never" class="boby-small" style="height: 100%">
        <div slot="header" class="clearfix">
          <span>工位任务状态</span>
        </div>
        <div style="margin-bottom: 10px">
          <el-row :gutter="4">
            <el-col :span="21">
              <el-input @keyup.enter.native="GetTasks" prefix-icon="el-icon-search" size="small" style="width: 300px;"
                class="filter-item" :placeholder="'PACK编码'" v-model="statusqueryListQuery.key"></el-input>
              <el-button type="primary" style="margin-left: 10px;" size="small" @click="GetTasks">查询</el-button>
              <!-- <el-button type="danger"
                             icon="el-icon-plus"
                             size="small"
                             @click="rework">返工</el-button>
                  <el-button type="danger"
                             icon="el-icon-plus"
                             size="small"
                             @click="moverealtime">返工完成</el-button> -->
            </el-col>
          </el-row>
        </div>
        <div>
          <el-table :data="statusqueryList" ref="dpTable" row-key="id" v-loading="statusqueryListLoading" border fit
            stripe highlight-current-row align="left">
            <el-table-column label="Pack条码" align="center" width="250" prop="packCode"></el-table-column>
            <el-table-column label="创建时间" align="center" prop="createTime"></el-table-column>
            <el-table-column label="完成工位" align="center" prop="station.code"></el-table-column>
            <!-- <el-table-column
              label="AGV车号"
              align="center"
              prop="useAGVCode"
            ></el-table-column> -->
            <el-table-column label="任务状态" align="center" prop="status">
              <template slot-scope="scope">
                <span>{{
                  scope.row.status == 2
                    ? "已完成"
                    : scope.row.status == 0
                      ? "未开始"
                      : "进行中"
                }}</span>
              </template></el-table-column>
            <!-- <el-table-column
              label="操作员"
              align="center"
              prop="createUser.name"
            ></el-table-column> -->
            <el-table-column label="操作" align="center" width="150">
              <template slot-scope="scope">
                <!-- <el-button size="mini" @click="ViewTaskDetails(scope.row)"  type="success">查看详情</el-button> -->

                <el-button size="mini" @click="SetCompleteTask(scope.row, 1)" v-show="scope.row.status != 2"
                  type="danger">设置完工</el-button>
                <el-button size="mini" @click="SetCompleteTask(scope.row, 0)" v-show="scope.row.status == 2"
                  type="danger">设置未完工</el-button>
                <!-- <el-button size="mini" @click="ClearBomDataById(scope.row)" type="danger">清除Bom数据</el-button>
                <el-button size="mini" @click="ClearDataById(scope.row)" type="danger">清除收数数据</el-button> -->
              </template>
            </el-table-column>
          </el-table>
          <pagination :total="statusqueryTotal" v-show="statusqueryTotal > 0" :page.sync="statusqueryListQuery.page"
            :limit.sync="statusqueryListQuery.limit" @pagination="handleCurrentChange" />
        </div>
      </el-card>
    </el-col>
  </div>
</template>

<script>
import * as statusquery from "@/api/statusquery";
import * as axios from "axios";
import waves from "@/directive/waves"; // 水波纹指令
import Sticky from "@/components/Sticky";
import permissionBtn from "@/components/PermissionBtn";
import Pagination from "@/components/Pagination";
import elDragDialog from "@/directive/el-dragDialog";
import * as rework from "@/api/rework";

export default {
  name: "statusquery",
  components: {
    Sticky,
    permissionBtn,
    Pagination,
    statusquery,
  },
  directives: {
    waves,
    elDragDialog,
  },
  data() {
    return {
      statusqueryList: [], // 列表
      statusqueryTotal: 0, //列表数据数目
      statusqueryListLoading: false, //主表加载特效
      statusqueryListQuery: {
        // 主表查询条件
        page: 1,
        limit: 20,
        key: undefined,
      },
    };
  },
  mounted() { },
  methods: {
    //分页
    handlestatusqueryCurrentChange(val) {
      this.statusqueryListQuery.page = val.page;
      this.statusqueryListQuery.limit = val.limit;
      this.loadDictionaryDetail();
    },
    handleCurrentChange() {
      if (this.statusqueryListQuery.key != null &&
        this.statusqueryListQuery.key.length > 0) {
        statusquery.load(this.statusqueryListQuery).then((response) => {
          this.statusqueryList = response.data; //提取数据
          this.statusqueryTotal = response.count; //提取数据的条数
        });
      }
    },
    //根据Pack查询
    GetTasks() {
      if (
        this.statusqueryListQuery.key == null ||
        this.statusqueryListQuery.key.length <= 0
      ) {
        this.$message({
          message: "请输入需要查询的PACK条码!",
          type: "error",
        });
        return;
      }
      this.statusqueryListLoading = true;
      statusquery.load(this.statusqueryListQuery).then((response) => {
        this.statusqueryList = response.data; //提取数据
        this.statusqueryTotal = response.count; //提取数据的条数
      });
      this.statusqueryListLoading = false;
    },

    //强制完工
    SetCompleteTask(val, finish) {

      var tipContext = finish == 1 ? "确定要设置完工吗？" : "确定要设置未完工吗？";

      this.$confirm(tipContext).then((_) => {
        var param = {
          id: val.id,
          status: finish
        };
        statusquery
          .changstatus(param)
          .then(() => {
            this.$notify({
              title: "成功",
              message: "已设置成功",
              type: "success",
              duration: 2000,
            });
            this.GetTasks(); //主表加载
          })
          .catch((_) => { });
      });
    },

    //强制完工
    ClearBomDataById(val, finish) {
      var tipContext = "确定要清除BOM数据吗?";
      this.$confirm(tipContext).then((_) => {
        var param = {
          id: val.id,
        };
        statusquery
          .ClearBomDataById(param)
          .then(() => {
            this.$notify({
              title: "成功",
              message: "清除BOM成功",
              type: "success",
              duration: 2000,
            });
            this.GetTasks(); //主表加载
          })
          .catch((_) => { });
      });
    },

    ClearDataById(val, finish) {
      var tipContext = "确定要清除收数数据吗?";
      this.$confirm(tipContext).then((_) => {
        var param = {
          id: val.id,
        };
        statusquery
          .ClearDataById(param)
          .then(() => {
            this.$notify({
              title: "成功",
              message: "清除收数数据成功",
              type: "success",
              duration: 2000,
            });
            this.GetTasks(); //主表加载
          })
          .catch((_) => { });
      });
    },
    //查看详情
    ViewTaskDetails(val) {
      const vm = this;

      vm.$router.push({
        path: "/taskDetails",
        query: {
          mainId: val.id
        }
      });
    },

    rework() {
      if (this.statusqueryListQuery.key == null ||
        this.statusqueryListQuery.key.length <= 0) {
        this.$message({
          message: "请输入需要返工的PACK条码!",
          type: "error",
        });
        return;
      }
      this.$confirm("确定要设置返工吗？").then((_) => {
        rework
          .movehistory({ packcode: this.statusqueryListQuery.key })
          .then(() => {
            this.$notify({
              title: "成功",
              message: "已设置成功",
              type: "success",
              duration: 2000,
            });
            this.GetTasks(); //主表加载
          })
          .catch((_) => { });
      });
    },
    moverealtime() {
      if (this.statusqueryListQuery.key == null ||
        this.statusqueryListQuery.key.length <= 0) {
        this.$message({
          message: "请输入需要返工完成的PACK条码!",
          type: "error",
        });
        return;
      }
      this.$confirm("确定要设置返工完成吗？").then((_) => {
        rework
          .moverealtime({ packcode: this.statusqueryListQuery.key })
          .then(() => {
            this.$notify({
              title: "成功",
              message: "已设置成功",
              type: "success",
              duration: 2000,
            });
            this.statusqueryList = null; //主表加载
          })
          .catch((_) => { });
      });
    },
  },
};
</script>

<style></style>
