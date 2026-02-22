<template>
  <div class="flex-column">
    <div class="filter-container">
      <el-card shadow="never" class="boby-small" style="height: 100%">
        <div slot="header" class="clearfix">
          <span>产品下线</span>
        </div>
        <div>
          <el-row :gutter="2" align="center">
            <el-col :span="3" >
              <el-date-picker
                clearable
                filterable
                size="small"
                class="filter-item"
                placeholder="开始日期"
                :type="datetype"
                v-model="productstatisticsQuery.begainTime"
                value-format="yyyy-MM-dd"
                @change="changeDate"
                :picker-options="pickerOptions0"
              ></el-date-picker> </el-col
            ><el-col :span="3" >
              <el-date-picker
                :clearable="false"
                filterable
                size="small"
                class="filter-item"
                placeholder="结束日期"
                :type="datetype"
                :editable="false"
                v-model="productstatisticsQuery.endTime"
                @change="changeDate"
                :picker-options="pickerOptions1"
                value-format="yyyy-MM-dd"
              >
              </el-date-picker> </el-col
            ><el-col :span="2">
              <el-select
                class="filter-item"
                size="small"
                v-model="productstatisticsQuery.productid"
                placeholder="请选择"
              >
                <el-option
                  v-for="item in products"
                  :key="item.id"
                  :label="item.name"
                  :value="item.id"
                >
                </el-option>
              </el-select>
            </el-col>
            
			<el-col :span="2" >
              <el-button
                type="infor"
                size="small"
                icon="el-icon-search"
                @click="Load"
                >搜索
              </el-button>
            </el-col>
          </el-row>
        </div>
      </el-card>
    </div>

    <div class="app-container fh">
      <el-table
        ref="mainTable"
        :data="productstatistics"
        v-loading="productstatisticsLoading"
        row-key="id"
        style="width: 100%"
        height="calc(100% - 52px)"
        @selection-change="handleSelectionChange"
        border
        fit
        stripe
        highlight-current-row
        align="left"
      >
        <el-table-column
          type="selection"
          min-width="20px"
          align="center"
        ></el-table-column>
        <el-table-column
          prop="productCode"
          label="产品条码"
          min-width="20px"
          sortable
          align="center"
        >
        </el-table-column>
        <el-table-column
          prop="instorageTime"
          label="下线时间"
          min-width="20px"
          sortable
          align="center"
        >
        </el-table-column>
        <!-- <el-table-column prop="description" label="描述" min-width="20px" sortable align="center"></el-table-column> -->
      </el-table>

      <pagination
        :total="total"
        v-show="total > 0"
        :page.sync="productstatisticsQuery.page"
        :limit.sync="productstatisticsQuery.limit"
        @pagination="handleCurrentChange"
      />
    </div>
  </div>
</template>

<script>
import * as proc_product_offline from "@/api/proc_product_offline";
import * as axios from "axios";
import waves from "@/directive/waves"; // 水波纹指令
import Sticky from "@/components/Sticky";
import * as product from "@/api/product";
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
      productstatistics: [], //数据表
      total: 0, //数据条数
      productstatisticsLoading: true, //加载特效
      search_visible: false,
      productstatisticsQuery: {
        //查询条件
        page: 1,
        limit: 20,
        key: undefined,
        productid: 1,
        begainTime: new Date(),
        endTime: new Date(),
      },
      listQuery: {
        beginTime: new Date(),
        endTime: new Date(),
        states: 1,
        productid: 0,
        key: "",
        limit: 0,
        page: 0,
      },
      stepTemp: {
        //模块临时值
        id: undefined,
        code: "",
        name: "",
        flowId: "",
        description: "",
      },

      dialogFormVisible: false, //编辑框
      dialogStatus: "", //编辑框功能(添加/编辑)
      textMap: {
        update: "编辑",
        create: "添加",
      },
      products: [],
      pickerOptions1: {},
      pickerOptions0: {},
      datetype: "date",
    };
  },
    created() {
    this.getProduct();
  },
  mounted() {
    this.Load();
  },
  methods: {
   
    changeDate() {
      // debugger
      //因为date1和date2格式为 年-月-日， 所以这里先把date1和date2转换为时间戳再进行比较
      let date1 = new Date(this.productstatisticsQuery.begainTime).getTime();

      let date2 = new Date(this.productstatisticsQuery.endTime).getTime();
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
    getDetails() {
      this.listLoading = true;
      this.detailquery.states=1;
      this.detailquery.productid=this.listQuery.productid
      this.detailquery.endTime=this.detailquery.beginTime;
      console.log(this.detailquery);
      proc_product_offline.GetListByProductId(this.detailquery).then((response) => {
        this.detaillist = response.data;
        this.total = response.count;
       
        console.log(this.detaillist);
        this.listLoading = false;
      });
    },
    //勾选框
    handleSelectionChange(val) {
      this.stepMultipleSelection = val;
    },
    //关键字搜索
    handleFilter() {
      this.Load();
    },
    //分页
    handleCurrentChange(val) {
      this.productstatisticsQuery.page = val.page;
      this.productstatisticsQuery.limit = val.limit;
      this.Load(); //页面加载
    },
    //列表加载
    Load() {
      this.productstatisticsLoading = true;

      proc_product_offline
        .Load(this.productstatisticsQuery)
        .then((response) => {
          this.productstatistics = response.data; //提取数据表

          this.total = response.count; //提取数据表条数
          this.productstatisticsLoading = false;
        });
    },
    //编辑框数值初始值
    resetTemp() {
      this.stepTemp = {
        id: undefined,
        code: "",
        name: "",
        flowId: "",
        description: "",
      };
    },
    //点击添加
    handleCreate() {
      //弹出编辑框
      this.resetTemp(); //数值初始化
      this.dialogStatus = "create"; //编辑框功能选择（添加）
      this.dialogFormVisible = true; //编辑框显示
      this.$nextTick(() => {
        this.$refs["dataForm"].clearValidate();
      });
    },
    getProduct() {
      product.getList().then((response) => {
        this.products = response.data;
        if (this.products.length>0) {
          this.listQuery.productid=this.products[0].id
          
        }
      });
    },
  },
};
</script>
