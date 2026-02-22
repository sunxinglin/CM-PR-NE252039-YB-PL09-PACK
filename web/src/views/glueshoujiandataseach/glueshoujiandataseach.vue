<template>
  <div style="height: 100%">
    <div class="filter-container">
      <el-card shadow="never" class="boby-small" style="height: 100%">
        <div slot="header" class="clearfix">
          <span>首件数据查询</span>
        </div>
        <div>
          <el-row :gutter="2">
            <el-col :span="21">
              <el-select v-model="taskTemp.code" placeholder="请选择">
                <el-option v-for="item in typeoptions" :key="item.id" :label="item.name" :value="item.code">
                </el-option>
              </el-select>
              <el-button type="primary" icon="el-icon-plus" size="small" @click="GetTasks">查询
              </el-button>
              <!-- <el-button type="primary" icon="el-icon-plus" size="small" @click="ModelExpornt">导出
              </el-button> -->
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
              <!-- 尝试肩部涂胶数据 -->
              <el-table :data="props.row.proc_StationTask_ShouJianDetails" style="width: 100%" max-height="600" stripe v-show="
                props.row.proc_StationTask_ShouJianDetails != null &&
                props.row.proc_StationTask_ShouJianDetails.length != 0
              ">
                <el-table-column label="首件参数名" prop="parameterName" align="center" />
                <el-table-column label="首件上传代码" prop="upMesCodePN" align="center" />
                <el-table-column label="首件参数值" prop="upValue" align="center" />
                <el-table-column label="工位" prop="glueStationCode" align="center" />
                <el-table-column label="开始时间" prop="createTime" align="center" />
                <el-table-column label="完成时间" prop="shouJianOverTime" align="center" />
              </el-table>
            </template>
          </el-table-column>
          <el-table-column label="工位" prop="glueStationCode" align="center" ></el-table-column>
          <el-table-column label="创建时间" prop="createTime" align="center" ></el-table-column>
          <el-table-column label="重传" min-width="100px" align="center">
						<template slot-scope="scope">
							<el-button-group>
								<el-button type="primary" icon="el-icon-document-add" size="small"
									@click="upgluedata(scope.row)">重传</el-button>
							</el-button-group>
						</template>
					</el-table-column>
        </el-table>
      </div>
    </div>
  </div>
</template>
<script>
import waves from "@/directive/waves"; // 水波纹指令
import * as stationGlueShouJian from "@/api/stationGlueShouJian";
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
   
      bingAGVPAck: {
					state: 0,
					agvcode: 0,
					stationcode: "",
					packpn: "",
					producttype: "",
					HolderBarCode: "",
				},
        typeoptions: [
        {
          id: 1,
          name:"下箱体涂胶",
          code: "2",
        },
        {
          id: 2,
          name:"间隙涂胶",
          code: "1",
        },
        {
          id: 3,
          name:"肩部涂胶",
          code: "3",
        },
        
      ],
      taskTemp: {
        //模块临时值
        id: undefined,
        //任务code
        code: "",
        //工位code
        stepcode: "",
        name: "",
        type: 1,
        stepId: this.stepId,
        productId:this.productId,
        hasPage: false,
        description: "",
        sequence: 1,
        resourceId: undefined,
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

      if (this.taskTemp.code == null || this.taskTemp.code <= 0) {
        this.$message({
          message: "请输入需要查询的PACK条码!",
          type: "error",
        });
        return;
      }
      this.stationListLoading = true;
      stationGlueShouJian.GetGuleShouJianData({ type: this.taskTemp.code }).then((response) => {
        console.log(response.data);
        if(response.data==null){
          this.stationListLoading = false;
        }
        else{
          this.tasklist = response.data;
        this.stationListLoading = false;
        console.log(this.tasklist);
        }
       
      });
    },
    ModelExpornt() {
      if (this.code ==null) {
        this.$message("请输入Pack码!");
        return;
      }
      /*traceback
        .ModelExpornt({pack:this.code})
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
        });*/
    },
   
      upgluedata(row) {
            console.log("请求"+row.id);
            stationGlueShouJian
            .UpLoadGlueDataCommAgain_ShouJian({ Id: row.id})
            .then((response) => {
                console.log("返回"+response);
                if (response.isError) {
                    this.$message({
                        message: "错误代码:" + response.errorCode + "\r\n" + "错误信息:" + response.errorMessage,
                        type: "error",
                    });
                    return;
                }
                this.$message({
                        message: "CATLMES返回正常",
                        type: "success",
                    });
                    return;
              
            });
        },
  },
};
</script>