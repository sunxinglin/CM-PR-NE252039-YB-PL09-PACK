<template>
  <div class="app-container">
    <el-col :span="24">
      <el-card shadow="never" class="boby-small" style="height: 100%">
        <div slot="header" class="clearfix">
          <span>超时检测数据查询</span>
        </div>
        <div style="margin-bottom: 10px">
          <el-row :gutter="4">
            <el-col :span="21">
              <el-input prefix-icon="el-icon-search" size="small" style="width: 300px" class="filter-item"
                :placeholder="'PACK编码'" v-model="pack"></el-input>
              <el-button type="primary" icon="el-icon-plus" size="small" @click="get">查询</el-button>
            </el-col>
          </el-row>
        </div>
        <div>
          <el-table :data="packList" ref="dpTable" row-key="id" v-loading="statusqueryListLoading" border fit stripe
            highlight-current-row align="left">
            <el-table-column label="Pack条码" align="center" width="500" prop="packPN"></el-table-column>
            <el-table-column label="名称" align="center" prop="gluingTimeName"></el-table-column>
            <el-table-column label="记录时间" align="center" prop="gluingTime"></el-table-column>
            <el-table-column label="日期" prop="createTime" align="center" />
            <!-- <el-table-column label="操作" align="center">
              <template slot-scope="scope">
              <el-button
              size="mini"
              @click="ViewTaskDetails(scope.row)"
              type="success"
              >查看详情</el-button
                >
              </template>
            </el-table-column> -->
            
          </el-table>
        </div>
      </el-card>
    </el-col>
    <el-dialog v-el-drag-dialog class="dialog-mini" width="500px" :visible.sync="glingtimeshow">
      <div>
        <el-form ref="dataForm1" :rules="gluingtimerules" :model="temp" label-position="right" label-width="100px">
          <el-form-item size="small" :label="'值'" prop="time">
            <el-input v-model="temp.time" :min="1" :max="20"></el-input>
          </el-form-item>
        </el-form>
      </div>
      <div slot="footer">
        <el-button size="mini" @click="glingtimeshow = false">取消</el-button>
        <el-button size="mini" type="primary" @click="savegluingtime">确认
        </el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import * as gluingtime from "@/api/gluingtime";
import * as axios from "axios";
import waves from "@/directive/waves"; // 水波纹指令
import Sticky from "@/components/Sticky";
import permissionBtn from "@/components/PermissionBtn";
import Pagination from "@/components/Pagination";
import elDragDialog from "@/directive/el-dragDialog";
import { time } from 'console';
export default {
  name: "gluingtime",
  components: {
    Sticky,
    permissionBtn,
    Pagination,
    gluingtime,
  },
  directives: {
    waves,
    elDragDialog,
  },
  data() {
    return {
      glingtimeshow: false, //弹窗
      statusqueryListLoading: false, //主表加载特效
      showDialog: false,
      pack: "",
      packList: [],
      temp: {
        id: 0,
        time: 0.0,
      },
      gluingtimerules: {
        time: [
          {
            required: true,
            message: "时间不能为空",
            trigger: "blur",
          },
        ],
      },
    };
  },
  mounted() { },
  methods: {
    resetTemp() {
      this.temp = {
        id: 0,
        time: 0.0,
      }
    },
    //根据Pack查询
    get() {
      this.statusqueryListLoading = true;
      var param = {
        pack: this.pack,
      };
      gluingtime.GetByPack(param).then((response) => {
        this.packList = response.data; //提取数据
      });
      this.statusqueryListLoading = false;
    },
    update(value) {
      this.resetTemp(),
        this.temp.id = value,
        this.glingtimeshow = true
    },
    ViewTaskDetails(val) {
      var incaseId = val.id;
      gluingtime.Updata({ dataId: incaseId }).then((response) => {
        if (response.code != 200) {
          this.$message({
            message: response.message,
            type: "error",
          });
          return;
        }
        this.detailData = response.result;
        this.dialogDetailVisible = true;
        //  console.log(this.detailData);
      });
    },
    savegluingtime() {
      gluingtime.Updata(this.temp).then(() => {
        this.$notify({
          title: "成功",
          message: "修改成功",
          type: "success",
          duration: 2000,
        });
        this.glingtimeshow = false;
        this.get();
      });
    }
  },
};
</script>

<style>

</style>
