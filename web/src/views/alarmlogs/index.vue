<template>
  <div class="flex-column">
    <el-card :className="'sub-navbar'">
      <div slot="header" class="clearfix">
        <span>错误日志</span>
      </div>
      <div>
        <el-row>
          <el-col :span="3">
            <el-date-picker clearable filterable size="small" class="filter-item" placeholder="开始日期" type="date"
              :editable="false" v-model="listQuery.beginTime" value-format="yyyy-MM-dd" @change="changeDate"
              :picker-options="pickerOptions0"></el-date-picker>
          </el-col>
          <el-col :span="3">
            <el-date-picker clearable filterable size="small" class="filter-item" placeholder="结束日期" type="date"
              :editable="false" v-model="listQuery.endTime" @change="changeDate" :picker-options="pickerOptions1"
              value-format="yyyy-MM-dd">
            </el-date-picker>
          </el-col>
             <el-col :span="3">
            <el-select class="filter-item" size="small" v-model="listQuery.stationCode" placeholder="请选择工位">
              <el-option v-for="item in stationlist" :key="item.key" :label="item.lable" :value="item.lable">
              </el-option>
            </el-select>
          </el-col>
          <el-col :span="3">
            <el-button type="infor" size="small" icon="el-icon-search" @click="handleFilter">搜索
            </el-button>
          </el-col>
        </el-row>
      </div>
    </el-card>
    <div class="app-container flex-item">
      <div class="bg-white" style="height: 100%">
        <el-table ref="mainTable" height="calc(100% - 52px)" :key="tableKey" :data="list" v-loading="listLoading" border
          fit highlight-current-row style="width: 100%" @row-click="rowClick">
          <el-table-column :label="'工位'" align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.stationCode }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'错误信息'" align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.description }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="'发生时间'" align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.occurTime }}</span>
            </template>
          </el-table-column>
          <el-table-column align="center" :label="'清除时间'">
            <template slot-scope="scope">
              <span>{{ scope.row.clearDate }}</span>
            </template>
          </el-table-column>
        </el-table>
        <pagination v-show="total > 0" :total="total" :page.sync="listQuery.page" :limit.sync="listQuery.limit"
          @pagination="handleCurrentChange" />
      </div>
    </div>
  </div>
</template>

<script>
import * as alarmlog from "@/api/alarmlog";
import waves from "@/directive/waves"; // 水波纹指令
import Sticky from "@/components/Sticky";
import  * as Station from "@/api/station";
import Pagination from "@/components/Pagination";
import elDragDialog from "@/directive/el-dragDialog";
import extend from "@/extensions/delRows.js";
export default {
  name: "sysLog",
  components: {
    Sticky,

    Pagination,
  },
  mixins: [extend],
  directives: {
    waves,
    elDragDialog,
  },
  data() {
    return {
      multipleSelection: [], // 列表checkbox选中的值
      tableKey: 0,
      list: null,
      total: 0,
      listLoading: true,
      listQuery: {
        // 查询条件
        page: 1,
        limit: 20,
        stationId: 0,
        stationCode:"",
        beginTime: new Date(),
        endTime: new Date(),
      },
      logtype: [
        {
          key: -1,
          lable: "全部",
        },
        {
          key: 0,
          lable: "用户登录",
        },
        {
          key: 1,
          lable: "新增用户",
        },
        {
          key: 2,
          lable: "修改用户",
        },
        {
          key: 3,
          lable: "新增角色",
        },
        {
          key: 4,
          lable: "修改角色",
        },
        {
          key: 5,
          lable: "删除角色",
        },
        {
          key: 6,
          lable: "角色配置",
        },
        {
          key: 7,
          lable: "权限配置",
        },
        {
          key: 8,
          lable: "新增产品",
        },
        {
          key: 9,
          lable: "修改产品",
        },
        {
          key: 10,
          lable: "删除产品",
        },
        {
          key: 11,
          lable: "新增工序",
        },
        {
          key: 12,
          lable: "修改工序",
        },
        {
          key: 13,
          lable: "删除工序",
        },
        {
          key: 14,
          lable: "新增工艺",
        },
        {
          key: 15,
          lable: "修改工艺",
        },
        {
          key: 16,
          lable: "删除工艺",
        },
        {
          key: 17,
          lable: "新增配方",
        },
        {
          key: 18,
          lable: "修改配方",
        },
        {
          key: 19,
          lable: "删除配方",
        },
        {
          key: 20,
          lable: "新增工位",
        },
        {
          key: 21,
          lable: "修改工位",
        },
        {
          key: 22,
          lable: "删除工位",
        },
      
      ],
      pickerOptions1: {},
      pickerOptions0: {},
      stationlist: [{
          key: 0,
          lable: "全部",
        },]
    };
  },
  mounted() {
    this.getList();
    this.getstation();
    this.changeDate();

  },
  methods: {
    rowClick(row) {
      this.$refs.mainTable.clearSelection();
      this.$refs.mainTable.toggleRowSelection(row);
    },


    getList() {
      this.listLoading = true;
      alarmlog.getList(this.listQuery).then((response) => {
        console.log(response.data);
        this.list = response.data;
        this.total = response.count;
        this.listLoading = false;
      });
    },
    handleFilter() {
      this.listQuery.page = 1;
      this.getList();
    },

    handleCurrentChange(val) {
      this.listQuery.page = val.page;
      this.listQuery.limit = val.limit;
      this.getList();
    },
    getstation() {
      Station.GetAllStation().then((response) => {
        response.result.forEach(element => {
          this.stationlist.push({"key":element.id,"lable":element.name})
        });
        console.log(this.stationlist);
      })
    },

    changeDate() {
      // debugger
      //因为date1和date2格式为 年-月-日， 所以这里先把date1和date2转换为时间戳再进行比较
      let date1 = new Date(this.listQuery.beginTime).getTime();

      let date2 = new Date(this.listQuery.endTime).getTime();
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
  },
};
</script>
<style>
.dialog-mini .el-select {
  width: 100%;
}
</style>
