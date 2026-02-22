<template>
  <div style="height: 100%">
    <div class="filter-container">
      <el-card shadow="never" class="boby-small" style="height: 100%">
        <div slot="header" class="clearfix">
          <span>正向追溯</span>
        </div>
        <div>
          <el-row :gutter="2">
            <el-col :span="21">
              <el-input @keyup.enter.native="GetTasks" prefix-icon="el-icon-search" size="small" style=" width: 20%"
                class="filter-item" :placeholder="'PACK编码'" v-model="code">
              </el-input>
              <el-button type="primary" icon="el-icon-plus" size="small" @click="GetTasks">查询
              </el-button>
              <el-button type="primary" icon="el-icon-plus" size="small" @click="ModelExpornt">导出
              </el-button>
            </el-col>
          </el-row>
        </div>
      </el-card>
      <div class="app-container fh">
        <el-table ref="maintaskTable" :data="tasklist" v-loading="stationListLoading" row-key="id" style="width: 100%"
          max-height="650" border fit stripe highlight-current-row align="center">
          <el-table-column type="expand">
            <!-- 一级下拉表 -->
            <template slot-scope="props">
              <!-- 尝试人工站数据 -->
              <el-table :data="props.row.proc_StationTask_Records" style="width: 100%" max-height="600" stripe v-show="props.row.proc_StationTask_Records != null &&
                props.row.proc_StationTask_Records.length != 0
                ">
                <el-table-column type="expand" style="height: 100%">
                  <!-- 二级下拉表 -->
                  <template slot-scope="scope">
                    <el-tabs>
                      <!-- 尝试人工扫码数据 -->
                      <el-table :data="scope.row.proc_StationTask_Boms" :height="300" border
                        v-if="scope.row.proc_StationTask_Boms != null && scope.row.proc_StationTask_Boms.length != 0">
                        <el-table-column type="expand" stripe>
                          <!-- 三级下拉表 -->
                          <template slot-scope="scopeDetail">
                            <el-table :data="scopeDetail.row.proc_StationTask_BomDetails
                              " stripe>
                              <el-table-column label="批次号" prop="batchBarCode" align="center" />
                              <el-table-column label="外部条码" prop="goodsOuterCode" align="center" />
                              <el-table-column label="库存码" prop="uniBarCode" align="center" />
                            </el-table>
                          </template>
                        </el-table-column>

                        <el-table-column label="物料名称" prop="goodsName" align="center" />
                        <el-table-column label="物料PN" prop="goodsPN" align="center" />

                        <el-table-column label="追溯方式" prop="tracingType" align="center">
                          <template slot-scope="scopeDetail">
                            <span>{{
                              scopeDetail.row.tracingType == "1"
                                ? "精追"
                                : scopeDetail.row.tracingType == 2
                                  ? "批追"
                                  : "扫库存"
                            }}</span>
                          </template>
                        </el-table-column>
                      </el-table>
                      <!-- 尝试人工拧螺丝数据 -->
                      <el-table :data="scope.row.proc_StationTask_BlotGuns" :height="300" border
                        v-if="scope.row.proc_StationTask_BlotGuns != null && scope.row.proc_StationTask_BlotGuns.length != 0">
                        <el-table-column type="expand">
                          <!-- 三级下拉表 -->
                          <template slot-scope="scopegun">
                            <el-table :data="scopegun.row.proc_StationTask_BlotGunDetails
                              ">
                              <el-table-column label="拧紧结果" prop="resultIsOK" align="center">
                                <template slot-scope="scope">
                                  <span>
                                    {{ scope.row.resultIsOK ? "正常" : "NG" }}
                                  </span>
                                </template>
                              </el-table-column>
                              <el-table-column label="程序号" prop="programNo" align="center" />
                              <el-table-column label="角度值" prop="finalAngle" align="center" />
                              <el-table-column label="扭矩值" prop="finalTorque" align="center" />
                              <el-table-column label="角度上传代码" prop="uploadCode" align="center" />
                              <el-table-column label="扭矩上传代码" prop="uploadCode_JD" align="center" />
                              <el-table-column label="使用设备" prop="base_ProResource.name" align="center" />
                              <el-table-column label="操作员" prop="createUser.name" align="center" />
                            </el-table>
                          </template>
                        </el-table-column>
                        <el-table-column label="螺丝型号" prop="screwName" align="center" />
                        <el-table-column label="当前已完成数" prop="curCompleteNum" align="center" />
                        <el-table-column label="需求总数" prop="useNum" align="center" />
                        <el-table-column label="程序号" prop="screwName" align="center" />
                        <el-table-column label="任务时间" prop="createTime" align="center" />
                        <el-table-column label="操作员" prop="createUser.name" align="center" />
                      </el-table>

                      <!-- 尝试人工称重数据 -->
                      <el-form v-model="scope.row.proc_StationTask_AnyLoad"
                        v-if="scope.row.proc_StationTask_AnyLoad != null" style="
                            display: inline-block;
                            vertical-align: middle;
                            width: 100%;">
                        <el-form-item label="名称"><span>{{
                          scope.row.proc_StationTask_AnyLoad.anyLoadName
                            }}</span>
                        </el-form-item>
                        <el-form-item label="重量"><span>{{
                          scope.row.proc_StationTask_AnyLoad.weightData
                            }}</span>
                        </el-form-item>
                        <el-form-item label="创建时间"><span>{{
                          scope.row.proc_StationTask_AnyLoad.createTime
                            }}</span>
                        </el-form-item>
                        <el-form-item label="操作员"><span>{{
                          scope.row.proc_StationTask_AnyLoad.createUser.name
                            }}</span>
                        </el-form-item>
                      </el-form>
                      <!-- 尝试人工扫描员工卡数据 -->
                      <el-form v-model="scope.row.proc_StationTask_ScanAccountCard"
                        v-if="scope.row.proc_StationTask_ScanAccountCard != null" style="
                            display: inline-block;
                            vertical-align: middle;
                            width: 100%;
                            margin-left: 2%;">
                        <el-form-item label="名称"><span>{{
                          scope.row.proc_StationTask_ScanAccountCard.scanAccountCardName
                            }}</span>
                        </el-form-item>
                        <el-form-item label="内容"><span>{{
                          scope.row.proc_StationTask_ScanAccountCard.accountValue
                            }}</span>
                        </el-form-item>
                        <el-form-item label="创建时间"><span>{{
                          scope.row.proc_StationTask_ScanAccountCard.createTime
                            }}</span>
                        </el-form-item>
                        <el-form-item label="操作员"><span>{{
                          scope.row.proc_StationTask_ScanAccountCard.createUser.name
                            }}</span>
                        </el-form-item>
                      </el-form>
                      <!-- 尝试用户输入数据 -->
                      <el-form v-model="scope.row.proc_StationTask_UserScan"
                        v-if="scope.row.proc_StationTask_UserScan != null" style="
                            display: inline-block;
                            vertical-align: middle;
                            width: 100%;
                            margin-left: 2%;">
                        <el-form-item label="名称"><span>{{
                          scope.row.proc_StationTask_UserScan.userScanName
                            }}</span>
                        </el-form-item>
                        <el-form-item label="输入内容"><span>{{
                          scope.row.proc_StationTask_UserScan.scanData
                            }}</span>
                        </el-form-item>
                        <el-form-item label="创建时间"><span>{{
                          scope.row.proc_StationTask_UserScan.createTime
                            }}</span>
                        </el-form-item>
                        <el-form-item label="操作员"><span>{{
                          scope.row.proc_StationTask_UserScan.createUser.name
                            }}</span>
                        </el-form-item>
                      </el-form>
                      <!-- 尝试扫码输入数据 -->
                      <el-form v-model="scope.row.proc_StationTask_ScanCollect"
                        v-if="scope.row.proc_StationTask_ScanCollect != null" style="
                            display: inline-block;
                            vertical-align: middle;
                            width: 100%;
                            margin-left: 2%;">
                        <el-form-item label="名称"><span>{{
                          scope.row.proc_StationTask_ScanCollect.scanCollectName
                            }}</span>
                        </el-form-item>
                        <el-form-item label="扫码内容"><span>{{
                          scope.row.proc_StationTask_ScanCollect.scanCollectData
                            }}</span>
                        </el-form-item>
                        <el-form-item label="创建时间"><span>{{
                          scope.row.proc_StationTask_ScanCollect.createTime
                            }}</span>
                        </el-form-item>
                        <el-form-item label="操作员"><span>{{
                          scope.row.proc_StationTask_ScanCollect.createUser.name
                            }}</span>
                        </el-form-item>
                      </el-form>
                      <!-- 尝试涂胶超时数据 -->
                      <el-form v-model="scope.row.proc_StationTask_GluingTime"
                        v-if="scope.row.proc_StationTask_GluingTime != null" style="
                            display: inline-block;
                            vertical-align: middle;
                            width: 100%;
                            margin-left: 2%;">
                        <el-form-item label="名称"><span>{{
                          scope.row.proc_StationTask_GluingTime.timeName
                            }}</span>
                        </el-form-item>
                        <el-form-item label="时长"><span>{{
                          scope.row.proc_StationTask_GluingTime.time
                            }}</span>
                        </el-form-item>

                        <el-form-item label="涂胶检测时间"><span>{{
                          scope.row.proc_StationTask_GluingTime.collectTime
                            }}</span>
                        </el-form-item>
                      </el-form>
                      <!-- 尝试静置时长数据 -->
                      <el-form v-model="scope.row.proc_StationTask_StewingTime"
                        v-if="scope.row.proc_StationTask_StewingTime != null" style="
                            display: inline-block;
                            vertical-align: middle;
                            width: 100%;
                            margin-left: 2%;">
                        <el-form-item label="Pack码"><span>{{
                          scope.row.proc_StationTask_StewingTime.serialCode
                            }}</span>
                        </el-form-item>
                        <el-form-item label="时间标志"><span>{{
                          scope.row.proc_StationTask_StewingTime.timeFlag
                            }}</span>
                        </el-form-item>
                        <el-form-item label="时间数据"><span>{{
                          scope.row.proc_StationTask_StewingTime.timeValue
                            }}</span>
                        </el-form-item>
                      </el-form>
                    </el-tabs>
                  </template>
                </el-table-column>

                <el-table-column label="任务名称" prop="taskName" align="center" />

                <el-table-column label="创建时间" prop="createTime" align="center" />

                <el-table-column label="任务状态" prop="status" align="center">
                  <template slot-scope="scope">
                    <span>{{
                      scope.row.status == 2
                        ? "已完成"
                        : scope.row.status == 0
                          ? "未开始"
                          : "进行中"
                    }}</span>
                  </template>
                </el-table-column>
              </el-table>

            </template>
          </el-table-column>
          <el-table-column label="PACK条码" prop="packCode" align="center" />
          <!-- <el-table-column label="任务序号" prop="curStepNo" align="center" /> -->
          <el-table-column label="创建时间" prop="createTime" align="center" />
          <el-table-column label="完成工位" prop="stationCode" align="center" />

          <!-- <el-table-column label="使用agv车号" prop="useAGVCode" align="center" /> -->
          <el-table-column label="任务状态" prop="status" align="center">
            <template slot-scope="scope">
              <span>{{
                scope.row.status == 2
                  ? "已完成"
                  : scope.row.status == 0
                    ? "未开始"
                    : "进行中"
              }}</span>
            </template>
          </el-table-column>
          <!-- <el-table-column label="操作员" prop="createUser.name" align="center" /> -->
        </el-table>
      </div>
    </div>
  </div>
</template>
<script>
import waves from "@/directive/waves"; // 水波纹指令
import * as traceback from "@/api/traceback";
export default {
  directives: {
    waves,
  },
  data() {
    return {
      tablegeight: null,
      tasklist: [],
      code: "",
      querydialogvisible: false,
      stationListLoading: false,
      outerParams: {
        无输入: 0,
        电压: 1,
        电阻: 2,
        电流: 3,
        压力: 4,
      },
    };
  },
  mounted() {
    let h = document.documentElement.clientHeight;
    let topH = this.$refs.maintaskTable.$el.offsetTop;
    this.tablegeight = (h - topH) * 0.81;
  },
  methods: {
    GetTasks() {
      this.tasklist = [];

      if (this.code == null || this.code.length <= 0) {
        this.$message({
          message: "请输入需要查询的PACK条码!",
          type: "error",
        });
        return;
      }
      this.stationListLoading = true;
      traceback.forwardgettask({ code: this.code }).then((response) => {
        console.log(response.data);
        if (response.data == null) {
          this.stationListLoading = false;
        }
        else {
          this.tasklist = response.data;
          this.stationListLoading = false;
          console.log(this.tasklist);
        }

      });
    },
    ModelExpornt() {
      if (this.code == null) {
        this.$message("请输入Pack码!");
        return;
      }
      traceback
        .ModelExpornt({ pack: this.code })
        .then((request) => {
          this.$notify({
            title: "提示",
            message: "数据整理完毕,正在下载",
            type: "success",
            duration: 2000,
          });
          var blob = new Blob([request], {
            type: "application/vnd.ms-excel",
          });
          var fileName = "Pack数据导出.xlsx";
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
  },
};
</script>