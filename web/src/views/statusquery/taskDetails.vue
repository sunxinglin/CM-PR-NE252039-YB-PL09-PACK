<template>
  <div class="app-container">
    <el-col :span="24">
      <el-card shadow="never" class="boby-small" style="height: 100%">
        <div slot="header" class="clearfix">
          <span>工位扫码详情</span>
        </div>
        <div>
          <span>{{ stationName }}</span>
          <span>{{ packCode }}</span>
        </div>
        <div>
          <el-table :data="tasksDetailList" ref="dpTable" row-key="id" v-loading="tasksDetailLoading" border fit stripe
            highlight-current-row align="left">
            <el-table-column label="序号" align="center" width="80" prop="orderNo"></el-table-column>
            <el-table-column label="任务名称" align="center" min-width="120" prop="taskName"></el-table-column>
            <el-table-column label="创建时间" align="center" min-width="120" prop="createTime"></el-table-column>
            <el-table-column label="任务状态" align="center" prop="status" width="120">
              <template slot-scope="scope">
                <span>{{
                  scope.row.status == 2
                    ? "已完成"
                    : scope.row.status == 0
                      ? "未开始"
                      : "进行中"
                }}</span>
              </template></el-table-column>
            <el-table-column label="追溯类型" align="center" width="100" prop="tracingType">
              <template slot-scope="scope">
                <span>{{
                  scope.row.tracingType == 0
                    ? "无"
                    : scope.row.tracingType == 1
                      ? "精追"
                      : scope.row.tracingType == 2
                        ? "批追"
                        : scope.row.tracingType == 3
                          ? "扫库存"
                          : "校验出货码"
                }}</span>
              </template></el-table-column>
            <el-table-column label="批次码" align="center" width="200" prop="batchBarCode"></el-table-column>
            <el-table-column label="精追码" align="center" width="300" prop="goodsOuterCode"></el-table-column>
            <el-table-column label="库存码" align="center" width="300" prop="uniBarCode"></el-table-column>
            <!-- <el-table-column label="是否已上传MES" align="center" width="80" prop="hasUpMesDone">
              <template slot-scope="scope">
                <el-button size="mini" @click="SetHasUpTask(scope.row, true)" v-show="scope.row.hasUpMesDone == false"
                  type="danger">未上传</el-button>
                <el-button size="mini" @click="SetHasUpTask(scope.row, false)" v-show="scope.row.hasUpMesDone == true"
                  type="success">已上传</el-button>
              </template></el-table-column> -->
            <!-- <el-table-column label="操作" align="center">
              <template slot-scope="scope">
                <el-button size="mini" @click="Delete(scope.row)" type="danger">踢料</el-button>
              </template>
            </el-table-column> -->
          </el-table>
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
export default {
  name: "taskDetails",
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
      tasksDetailList: [], // 列表
      tasksDetailTotal: 0, //列表数据数目
      tasksDetailLoading: false, //主表加载特效
      stationName: "",
      packCode: "",
      mainId: 0,
    };
  },
  mounted() {
    this.mainId = this.$route.query.mainId;
    console.log(this.$route.query.mainId);
    this.GetTasksDetail(this.mainId)
  },

  methods: {
    //根据Pack查询
    GetTasksDetail(val) {
      this.tasksDetailLoading = true;
      var param = {
        id: val
      };
      statusquery.loadStationTaskDetail(param).then((response) => {
        this.stationName = response.stationCode;
        this.packCode = response.packCode;
        this.tasksDetailList = response.data.proc_StationTask_Record_DTO_List; //提取数据
        this.tasksDetailTotal = response.count; //提取数据的条数
      });
      this.tasksDetailLoading = false;
    },

    //踢料
    Delete(val) {
      var tipContext = "确定要踢料吗？";
      this.$confirm(tipContext).then((_) => {
        var param = {
          id: val.id,
        };
        console.log(param);
        statusquery
          .DeleteScan(param)
          .then(() => {
            this.$notify({
              title: "成功",
              message: "已踢料成功",
              type: "success",
              duration: 2000,
            });
            this.GetTasksDetail(this.mainId); //加载
          })
          .catch((_) => { });
      });
    },
    //强制已上传
    SetHasUpTask(val, finish) {
      var tipContext = finish == false ? "确定要设置【未上传】吗？" : "确定要设置【已上传】吗？";

      this.$confirm(tipContext).then((_) => {
        var param = {
          id: val.id,
          status: finish
        };
        statusquery
          .ChangeHasUp(param)
          .then(() => {
            this.$notify({
              title: "成功",
              message: "已设置成功",
              type: "success",
              duration: 2000,
            });
            this.GetTasksDetail(this.mainId); //加载
          })
          .catch((_) => { });
      });
    },
  },
};
</script>

<style></style>
