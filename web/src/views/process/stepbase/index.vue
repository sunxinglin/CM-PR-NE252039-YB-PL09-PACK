<template>
  <div class="flex-column">
    

    <div class="app-container fh">
      <el-row :gutter="20">
        <el-col :span="12">
          <el-table ref="mainStepTable" title="工序列表" :data="stepList" v-loading="stepListLoading" row-key="id" style="width: 100%"
        :height="tableHeight" border fit stripe highlight-current-row  @current-change="handleCurrentChange"
            align="left">
        <el-table-column prop="code" label="工序编号" min-width="120px" sortable align="center"></el-table-column>
        <el-table-column prop="name" label="工序名称" min-width="150px" sortable align="center"></el-table-column>
        <el-table-column prop="steptype" label="类型" min-width="300px" :formatter="changeStatus" align="center"></el-table-column>

      </el-table>
        </el-col>
        <el-col :span="12">
          <el-table ref="mainStationTable" :data="stationList" v-loading="stationListLoading" row-key="id" style="width: 100%"
        :height="tableHeight"  border fit stripe highlight-current-row
        align="left">
        <el-table-column prop="code" label="工位编号" min-width="120px" sortable align="center"></el-table-column>
        <el-table-column prop="name" label="工位名称" min-width="150px" sortable align="center"></el-table-column>
        <el-table-column prop="description" label="描述" min-width="300px" sortable align="center"></el-table-column>
      </el-table>
        </el-col>
      </el-row>
    </div>

  </div>

</template>

<script>
import * as steps from "@/api/step";
import * as stations from "@/api/station";
import * as dictionaryDetails from "@/api/dictionaryDetail";
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
  },
  directives: {
    waves,
    elDragDialog,
  },
  data() {
    return {
      tableHeight: null,
      stepList: [], //数据表
      stationList: [], //数据表
      stepId:0,
      stepTotal: 0, //数据条数
      stepListLoading: true, //加载特效
      stationListLoading: false, //加载特效
      
      stepListQuery: {
        //查询条件
        page: 1,
        limit: 200,
        key: undefined,
      },

      stationListQuery: {
        stepId:0
      },

      
      stepTemp: {
        //模块临时值
        id: undefined,
        code: "",
        name: "",
        steptype: 0,
      },
      typeOptions: [],
    };
  },
  mounted() {
    let h = document.documentElement.clientHeight; // 可见区域高度
    let topH = this.$refs.mainStepTable.$el.offsetTop; //表格距浏览器顶部距离
    let tableHeight = (h - topH) -100; //表格应该有的高度   乘以多少可自定义
    this.tableHeight = tableHeight;
    this.getstepType();
    
  },
  methods: {
    changeStatus(row, column, cellValue) {
      var curRowType= this.typeOptions.filter(function(item){
        return item.key==cellValue;
      });
      return curRowType[0].display_name;
    },

    //勾选框
    handleCurrentChange(val) {
      this.stationList=[]
      this.stepId=val.id;
      this.stationListQuery.stepId=val.id;
      this.stationListLoading = true;
      stations.GetStationsByStepId(this.stationListQuery).then((response) => {
        this.stationList = response.data; //提取数据表
        this.stationTotal = response.count; //提取数据表条数
        this.stationListLoading = false;
      });
    },

    //列表加载
    stepLoad() {
      this.stepListLoading = true;
      steps.load(this.stepListQuery).then((response) => {
        this.stepList = response.data; //提取数据表
        this.stepTotal = response.count; //提取数据表条数
        this.stepListLoading = false;
      });
    },
    getstepType() {
      var _this = this; // 记录vuecomponent
      var param = {
        typeCode: "stepType",
      };
      dictionaryDetails.getListByType(param).then((response) => {
        _this.types = response.data.map(function (item) {
          return {
            key: item.value,
            display_name: item.name,
          };
        });
        _this.typeOptions = JSON.parse(JSON.stringify(_this.types));
        this.stepLoad();
      });
    },
  },



};
</script>
