<template>
  <!-- 正在追朔页面 -->
  <div class="flex-column">
    <div class="filter-container">
      <el-card shadow="never" class="boby-small" style="height: 100%">
        <div slot="header" class="clearfix">
          <span>产品NG下线饼状图</span>
        </div>
        <div>
          <el-row :gutter="2">
            <el-col :span="3">
              <el-date-picker clearable filterable size="small" class="filter-item" placeholder="开始日期" type="date"
                :editable="false" v-model="listQuery.beginTime" value-format="yyyy-MM-dd" @change="changeDate"
                :picker-options="pickerOptions0"></el-date-picker> </el-col><el-col :span="3">
              <el-date-picker clearable filterable size="small" class="filter-item" placeholder="结束日期" type="date"
                :editable="false" v-model="listQuery.endTime" @change="changeDate" :picker-options="pickerOptions1"
                value-format="yyyy-MM-dd">
              </el-date-picker>
            </el-col>
            <!-- <el-col :span="3">
              <el-select
                class="filter-item"
                size="small"
                v-model="listQuery.productid"
                placeholder="请选择产品"
              >
                <el-option
                  v-for="item in products"
                  :key="item.id"
                  :label="item.name"
                  :value="item.id"
                >
                </el-option>
              </el-select>
            </el-col> -->
            <el-col :span="3" style="padding-left :20px">
              <el-button type="infor" size="small" icon="el-icon-search" @click="getlist">搜索
              </el-button>
            </el-col>
          </el-row>
        </div>
      </el-card>
    </div>
    <div class="app-container fh">
      <div class="bg-white" style="height: 78vh">
        <div id="myChart" :style="{ width: '100%', height: '78vh' }"></div>
      </div>
    </div>

    <el-dialog class="dialog-mini" :visible.sync="dialogVisible" width="80%" style="height: 95vh"
      :close-on-click-modal="false">
      <sticky :className="'sub-navbar'">
        <div class="filter-container">
          <el-row>
            <el-col>

              <!-- 筛选的搜索框 -->
              <div style="margin-bottom: 10px; display: inline">
                <el-date-picker clearable filterable size="small" class="filter-item" placeholder="开始日期" type="date"
                  :editable="false" v-model="detailquery.beginTime" value-format="yyyy-MM-dd" @input="changeDetailDate"
                  :picker-options="pickerOptions2"></el-date-picker>

                <el-date-picker clearable filterable size="small" class="filter-item" placeholder="结束日期" type="date"
                  :editable="false" v-model="detailquery.endTime" @input="changeDetailDate"
                  :picker-options="pickerOptions3" value-format="yyyy-MM-dd">
                </el-date-picker>
              </div>
              <el-button size="mini" @click="getDetails" icon="el-icon-search">搜索</el-button>
            </el-col>
          </el-row>
        </div>
      </sticky>
      <div class="app-container" style="height: 65vh">
        <div class="bg-white" height="100vh">
          <el-table ref="mainTable" :data="detaillist" v-loading="listLoading" border fit stripe highlight-current-row
            style="width: 100%" height="54vh" @selection-change="handleSelectionChange" align="left">
            <el-table-column :show-overflow-tooltip="true" min-width="100px" label="下线时间" align="center"
              prop="instorageTime" sortable>
            </el-table-column>
            <el-table-column :show-overflow-tooltip="true" min-width="80px" label="产品条码" align="center"
              prop="productCode" sortable>
            </el-table-column>

            <el-table-column :show-overflow-tooltip="true" min-width="100px" label="所属产品" align="center"
              prop="product.name" sortable>
            </el-table-column>
            <el-table-column :show-overflow-tooltip="true" min-width="100px" label="下线工位" align="center"
              prop="station.name" sortable>
            </el-table-column>
          </el-table>
          <pagination v-show="total > 0" :total="total" :page.sync="detailquery.page" :limit.sync="detailquery.limit"
            @pagination="getDetails" />
        </div>
      </div>
    </el-dialog>
  </div>
</template>
<script>
import Sticky from "@/components/Sticky";
import Pagination from "@/components/Pagination";
import * as proc_product_offline from "@/api/proc_product_offline";
import * as product from "@/api/product";
import permissionBtn from "@/components/PermissionBtn";

export default {
  name: "ProjecTask",
  components: {
    Sticky,
    Pagination,
    permissionBtn,
  },
  data() {
    return {
      listLoading: true,
      total: 0,
      chartlist: [],
      goodsListTemp: [],
      multipleSelection: [],
      dialogVisible: false,
      detaillist: [],
      procedureList: [],
      goodsList: [],
      listQuery: {
        beginTime: new Date(),
        endTime: new Date(),
        states: 2,
        productid: 0,
        key: "",
        limit: 0,
        page: 0,
      },
      detailquery: {
        beginTime: new Date(),
        endTime: new Date(),
        states: 2,
        productid: 0,
        key: "",
        limit: 0,
        page: 0,
      },
      datetype: {
        name: "",
        value: "",
      },
      products: [],
      pickerOptions1: {},
      pickerOptions0: {},
      pickerOptions2: {},
      pickerOptions3: {},
    };
  },
  computed: {},
  filters: {},
  created() {
    // this.getProduct();
  },
  mounted() {
    this.dateinit();
    this.changeDate();
  },
  methods: {
    // buttenselect(){
    //    getdetails();
    // },
    handleFilter() {
      this.getlist();
    },
    drawLine(data, lablist) {
      var _this = this;
      // 基于准备好的dom，初始化echarts实例
      let myChart = this.$echarts.init(document.getElementById("myChart"));
      // 绘制图表
      myChart.setOption({
        legend: {
          // 图例
          data: lablist,
          right: "10%",
          top: "30%",
          orient: "vertical",
        },

        title: {
          text: "PACK NG下线产品分析图",
          top: "0%",
          left: "center",
        },
        xAxis: {},
        yAxis: {},
        series: [
          {

            type: "pie",

            data: data,
            itemStyle: {
              normal: {
                label: {
                  show: true,
                  formatter: "{b} : {c} ({d}%)",
                },
                labelLine: { show: true },
              },
            },
          },
        ],
      });
      //防止重复触发点击事件
      if (myChart._$handlers.click) {
        myChart._$handlers.click.length = 0;
      }
      window.addEventListener("resize", () => {
        myChart.resize();
      });
      myChart.on("click", function (params) {
        // 控制台打印数据的名称
        // this.listQuery.selectDate=params.name;
        _this.chartclick(params);
      });
    },
    chartclick(params) {
      this.resetFilter1();
      this.detailquery.beginTime = this.listQuery.beginTime;
      this.detailquery.endTime = this.listQuery.endTime;
      this.detailquery.productid = params.data.id;
      this.dialogVisible = true;
      this.getDetails();
    },
    resetFilter1() {
      this.detailquery = {
        // 查询条件
        page: 1,
        limit: 10,
        key: "",
        productid: "",
        states: 2
      };
    },
    getlist() {
      proc_product_offline.GetRoundCakeData(this.listQuery).then((response) => {
        this.chartlist = response.result;
        console.log(this.chartlist);

        var datelist = this.chartlist.map((x) => {
          var date = {};
          date.name = x.productName;
          date.value = x.percentage;
          date.id = x.productId;
          return date;
        });
        var lablist = this.chartlist.map((o) => o.productName);
        console.log(datelist);
        this.drawLine(datelist, lablist);
      });
    },
    getDetails() {
      this.listLoading = true;


      proc_product_offline
        .GetListByProductId(this.detailquery)
        .then((response) => {
          this.detaillist = response.data;
          this.total = response.count;

          console.log(this.detaillist);
          this.listLoading = false;
        });
    },

    handleSelectionChange(val) {
      this.multipleSelection = val;
    },
    rowClickBtn(row) {
      this.dialogVisible = true;
      this.goodsListTemp = row;
      console.log(1, this.goodsListTemp);

      this.getGoodsList(row.id);
      this.getQuipmentList(row.id);
    },

    handleDetails() {
      this.dialogVisible = true;
    },

    handleClose(tag) {
      if (tag.id == "beginTime") {
        this.listQuery.beginTime = undefined;
      }
      if (tag.id == "endTime") {
        this.listQuery.endTime = undefined;
      }
      this.listQuery.page = 1;
      this.getList();
    },
    dateinit() {
      this.listQuery.endTime = new Date();
      // this.listQuery.beginTime=new Date().setDate( new Date().getDate()-7);
      var now = new Date();
      now = now.getTime();

      this.listQuery.beginTime = new Date(now - 30 * 24 * 3600 * 1000);
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
    changeDetailDate() {
      // debugger
      //因为date1和date2格式为 年-月-日， 所以这里先把date1和date2转换为时间戳再进行比较
      let date1 = new Date(this.detailquery.beginTime).getTime();

      let date2 = new Date(this.detailquery.endTime).getTime();
      this.pickerOptions2 = {
        disabledDate: (time) => {
          if (date2 != "") {
            return time.getTime() >= date2;
          }
        },
      };
      this.pickerOptions3 = {
        disabledDate: (time) => {
          return time.getTime() <= date1;
        },
      };
    },
    resetFilter() {
      this.listQuery = {
        // 查询条件
        page: 1,
        limit: 10,
        ProcNo: 0,
        key: undefined,
        status: undefined,
        type: undefined,
        beginTime: undefined,
        endTime: undefined,
      };

      this.handleFilter();
      this.dateinit();
    },
    getProduct() {
      product.getList().then((response) => {
        this.products = response.data;
        if (this.products.length > 0) {
          this.listQuery.productid = this.products[0].id;
        }
      });
    },
  },
};
</script>
