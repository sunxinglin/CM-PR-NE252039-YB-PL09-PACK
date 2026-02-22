<template>
  <div class="flex-column">
    <div class="filter-container">
      <el-card shadow="never" class="boby-small" style="height: 100%">
        <div slot="header" class="clearfix">
          <span>工艺路线配置</span>
        </div>
        <div>
          <el-row :gutter="2">
            <el-col :span="21">
              <el-button type="primary" icon="el-icon-plus" size="small" @click="handleCreate">添加
              </el-button>
              <el-button type="primary" icon="el-icon-edit" size="small" @click="handleUpdate">编辑
              </el-button>
              <el-button type="danger" icon="el-icon-delete" size="small" @click="handleDelete">
                删除</el-button>
            </el-col>
            <el-col :span="3">
              <el-input @keyup.enter.native="handleFilter" prefix-icon="el-icon-search" size="small"
                style="width: 200px" class="filter-item" :placeholder="'工艺编码/名称'" v-model="listQuery.key" clearable>
              </el-input>
            </el-col>
          </el-row>
        </div>
      </el-card>
    </div>

    <div class="app-container fh">
      <el-table ref="mainTable" :data="list" v-loading="listLoading" row-key="id" border fit stripe
        highlight-current-row style="width: 100%" height="calc(100% - 52px)" @selection-change="handleSelectionChange"
        align="left">
        <el-table-column type="selection" align="center" width="55"></el-table-column>
        <el-table-column min-width="20px" :label="'工艺编码'" prop="code" sortable align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.code }}</span>
          </template>
        </el-table-column>
        <el-table-column min-width="20px" :label="'工艺名称'" prop="name" sortable align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.name }}</span>
          </template>
        </el-table-column>
        <el-table-column min-width="20px" :label="'产品PN'" prop="productId" sortable align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.product.name }}</span>
          </template>
        </el-table-column>

        <el-table-column min-width="20px" :label="'描述'" prop="description" sortable align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.description }}</span>
          </template>
        </el-table-column>
      </el-table>

      <pagination v-show="total > 0" :total="total" :page.sync="listQuery.page" :limit.sync="listQuery.limit"
        @pagination="handleCurrentChange" />
    </div>

    <el-dialog v-el-drag-dialog class="dialog-mini" width="80%" height="600px" :title="textMap[dialogStatus]"
      :visible.sync="dialogFormVisible">
      <div style="height: 650px;">
        <el-form :rules="rules" ref="dataForm" :model="temp" label-position="right" label-width="100px" size="small"
          style="height: 100%;">
          <el-row :gutter="0">
            <el-col :span="6">
              <el-form-item :label="'工艺编码'" prop="code">
                <el-input v-model="temp.code" v-bind:disabled="dialogStatus != 'create'" width="176px">
                </el-input>
              </el-form-item>
            </el-col>
            <el-col :span="6">
              <el-form-item :label="'工艺名称'" prop="name">
                <el-input v-model="temp.name" width="176px"></el-input>
              </el-form-item>
            </el-col>

            <el-col :span="8">
              <el-form-item :label="'产出品'" prop="productId">
                <el-select class="filter-item" style="width: 250px" filterable remote reserve-keyword
                  v-model="temp.productId" placeholder="Please select" @change="productSelectchange">
                  <el-option v-for="item in productOptions" :key="item.key" :label="item.display_name"
                    :value="item.key"></el-option>
                </el-select>
              </el-form-item>
            </el-col>
          </el-row>

          <el-row :gutter="0">
            <el-col :span="24">
              <el-form-item :label="'描述'" prop="description">
                <el-input v-model="temp.description"></el-input>
              </el-form-item>
            </el-col>
          </el-row>

          <el-row>
            <label style="font-size: 15px">以下是工序列表 </label>
            <el-button type="primary" size="small" @click="handleDetailCreate()">添加工序
            </el-button>
          </el-row>

          <el-row>

            <template>
              <el-table ref="tableData" :data="dialogChildList" style="width: 100%" :height="dialogtabheight"
                max-height="500px" min-height="120px" border>
                <el-table-column align="center" label="序号" min-width="60px" prop="orderNo">
                </el-table-column>
                <el-table-column align="center" label="工序" min-width="220px" prop="stepId">
                  <template slot-scope="scope">
                    <el-select size="small" class="filter-item" style="width: 100%" learable filterable remote
                      reserve-keyword v-model="scope.row.stepId" placeholder="请选择" @change="ChangeSelect">
                      <el-option v-for="item in stepOptions" :key="item.key" :disabled="item.hasSelect"
                        :label="item.display_name" :value="item.key"></el-option>
                    </el-select>
                  </template>
                </el-table-column>
                <!-- <el-table-column align="left" label="工序名" min-width="350px" prop="stepName">
												<template slot-scope="scope">
													<el-input size="small" class="filter-item" style="width: 100%" v-model="scope.row.stepName"
														placeholder="请输入工序名">
													</el-input>

												</template>
											</el-table-column> -->
                <el-table-column fixed="right" align="center" class-name="small-padding fixed-width" label="操作"
                  width="300px">
                  <template slot-scope="scope">
                    <el-button-group>
                      <el-button size="small" plain type="success" icon="el-icon-top"  @click.native.prevent="
  handleDetailUpDown(
    scope.row.id,
    scope.$index,
    dialogChildList,
    -1,
    $event
  )
"></el-button>
                      <el-button size="small" plain type="success" icon="el-icon-bottom"  @click.native.prevent="
  handleDetailUpDown(
    scope.row.id,
    scope.$index,
    dialogChildList,
    1,
    $event
  )
"></el-button>
                      <el-button size="small" plain type="danger" icon="el-icon-delete" @click.native.prevent="
                        handleDetailDelete(
                          scope.row.id,
                          scope.$index,
                          dialogChildList
                        )
                      "></el-button>
                    </el-button-group>

                    <!-- <el-button size="small" type="danger"
														@click.native.prevent="handleDetailDelete(scope.row.id,scope.$index, dialogChildList)">
														删除
													</el-button> -->
                  </template>
                </el-table-column>
              </el-table>
            </template>

          </el-row>
        </el-form>
      </div>
      <div slot="footer">
        <el-button size="small" @click="dialogFormVisible = false">取消</el-button>
        <el-button size="small" v-if="dialogStatus == 'create'" type="primary" @click="createData">确认
        </el-button>
        <el-button size="small" v-else type="primary" @click="updateData">确认</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import * as flows from "@/api/flow";
import * as steps from "@/api/step";
import * as products from "@/api/product";

import waves from "@/directive/waves"; // 水波纹指令

import Pagination from "@/components/Pagination";
import elDragDialog from "@/directive/el-dragDialog";
export default {
  name: "ProFlow",

  components: {

    Pagination,
  },
  directives: {
    waves,
    elDragDialog,
  },
  data() {
    return {
      multipleSelection: [], //勾选的数据表值
      list: [], //数据表
      total: 0, //数据条数
      listLoading: true, //加载特效
      listQuery: {
        //查询条件
        page: 1,
        limit: 20,
        key: undefined,
      },
      dialogtabheight: 0,

      stepOptions: [],
      productOptions: [],
      temp: {
        //模块临时值
        id: undefined,
        code: "",
        name: "",
        ProductId: undefined,
        stepList: [],
        description: "",
      },
      dialogChildList: [],
      dialogFormVisible: false, //编辑框
      dialogStatus: "", //编辑框功能(添加/编辑)
      textMap: {
        update: "编辑",
        create: "添加",
      },
      rules: {
        //编辑框输入限制
        code: [
          {
            required: true,
            message: "编号不能为空",
            trigger: "blur",
          },
        ],
        name: [
          {
            required: true,
            message: "名称不能为空",
            trigger: "blur",
          },
        ],
        productId: [
          {
            required: true,
            message: "请选择产出品",
            trigger: "blur",
          },
        ],
      },
    };
  },
  mounted() {
    this.getList();
    // this.getStepList();
    this.getProductList();
  },

  methods: {
    productSelectchange(val){
      this.getStepList(val);
    },
    //勾选框
    handleSelectionChange(val) {
      this.multipleSelection = val;
    },
    //关键字搜索
    handleFilter() {
      this.getList();
    },
    //分页
    handleCurrentChange(val) {
      this.listQuery.page = val.page;
      this.listQuery.limit = val.limit;
      this.getList(); //页面加载
    },
    //列表加载
    getList() {
      this.listLoading = true;
      flows.load(this.listQuery).then((response) => {
        this.list = response.data; //提取数据表
        this.total = response.count; //提取数据表条数
        this.listLoading = false;
      });
    },
    //编辑框数值初始值
    resetTemp() {
      this.temp = {
        id: undefined,
        code: "",
        name: "",
        ProductId: undefined,
        stepList: [],
        description: "",
      };
    },
    //点击添加
    handleCreate() {
      //弹出编辑框
      this.resetTemp(); //数值初始化
      this.dialogChildList = [];
      this.dialogStatus = "create"; //编辑框功能选择（添加）
      this.dialogFormVisible = true; //编辑框显示
      this.dialogtabheight="500px"
      this.$nextTick(() => {
        this.$refs["dataForm"].clearValidate();
      });
    },
    //保存提交
    createData() {
      this.$refs["dataForm"].validate((valid) => {
        if (valid) {
          if (!this.checkupsteplist()) {
            this.$message({
              message: "工序需要填写完成",
              type: "error",
            });
            return;
          }
          this.temp.stepList = this.dialogChildList;
          flows.add(this.temp).then(() => {
            this.dialogFormVisible = false; //编辑框关闭
            this.$notify({
              title: "成功",
              message: "创建成功",
              type: "success",
              duration: 2000,
            });
            this.getList(); //页面加载
          });
        }
      });
    },
    //点击编辑
    handleUpdate() {
      if (this.multipleSelection.length !== 1) {
        this.$message({
          message: "只能选中一个进行编辑",
          type: "error",
        });
        return;
      } else {
        var row = this.multipleSelection[0];
        //弹出编辑框
        this.temp = Object.assign({}, row); //复制选中的数据
        this.dialogStatus = "update"; //编辑框功能选择（更新）
        this.dialogFormVisible = true; //编辑框显示
        var param = {
          flowId: row.id,
        };
        flows.getChildList(param).then((response) => {
          this.dialogtabheight = (response.data.length != 0 ? (response.data.length+2) * 34 : 500) 
          if (this.dialogtabheight<=500) {
           this.dialogtabheight=500
          }
     
          this.dialogtabheight=this.dialogtabheight + "px"
          console.log(this.dialogtabheight);
          this.dialogChildList = response.data;
          this.refreshSelect();
        });
        this.getStepList(row.productId);
        this.$nextTick(() => {
          this.$refs["dataForm"].clearValidate();
        });
      }
    },
    //更新提交
    updateData() {
      this.$refs["dataForm"].validate((valid) => {
        if (valid) {
          if (!this.checkupsteplist()) {
            this.$message({
              message: "工序需要填写完成",
              type: "error",
            });
            return;
          }
          this.temp.stepList = this.dialogChildList;
          flows.update(this.temp).then(() => {
            this.dialogFormVisible = false; //编辑框关闭
            this.$notify({
              title: "成功",
              message: "更新成功",
              type: "success",
              duration: 2000,
            });
            this.getList(); //页面加载
          });
        }
      });
    },
    checkupsteplist() {
      var state = 0;
      this.dialogChildList.forEach((element) => {
        if (element.stepId.length == 0) {
          state++;
        }
        this.stepOptions.forEach((item) => {
          if (element.stepId == item.key) {
            if (item.steptype == 2) {
              //element.orderNo = 999;
            }
          }
        });
      });
      if (state == 0) {
        return true;
      } else {
        return false;
      }
    },
    //点击删除
    handleDelete() {
      if (this.multipleSelection.length < 1) {
        this.$message({
          message: "至少删除一个",
          type: "error",
        });
        return;
      }
      this.$confirm("确定要删除吗？")
        .then(() => {
          var rows = this.multipleSelection;
          var selectids = rows.map((u) => u.id); //提取复选框的数据的Id
          var param = {
            ids: selectids,
          };
          flows.del(param).then(() => {
            this.$notify({
              title: "成功",
              message: "删除成功",
              type: "success",
              duration: 2000,
            });
            this.getList(); //页面加载
          });
        })
        .catch(() => { });
    },
    getStepList(productId) {
      var _this = this; // 记录vuecomponent
     
      var param = {"key":null};
      steps.getList(param).then((response) => {
        _this.steps = response.data.map(function (item) {
          return {
            key: item.id,
            display_name: item.code,
            steptype: item.steptype,
          };
        });
        _this.stepOptions = JSON.parse(JSON.stringify(_this.steps));

        _this.refreshSelect();
      });
    },
    getProductList() {
      var _this = this; // 记录vuecomponent
      var param = {};
      products.getList(param).then((response) => {
        _this.products = response.data.map(function (item) {
          return {
            key: item.id,
            display_name: item.name,
          };
        });
        _this.productOptions = JSON.parse(JSON.stringify(_this.products));
      });
    },
    handleDetailCreate() {
      // 添加明细
      var inlinelist = this.dialogChildList.filter((a) => {
        if (a.orderNo != 999) {
          return a;
        }
      });

      this.dialogChildList.push({
        id: 0,
        flowId: this.temp.id,
        orderNo: inlinelist.length == 0 ? 1 : inlinelist[inlinelist.length - 1].orderNo + 1,
        stepId: "",
        canchange: true,
      });
      //   this.dialogChildList.sort(function(a,b){return a-b});
      this.refreshSelect();

      this.$nextTick(() => {
        this.$refs.tableData.bodyWrapper.scrollTop = this.$refs.tableData.bodyWrapper.scrollHeight
      })

      console.log(this.dialogChildList.length);

    },

    handleDetailDelete(id, index, rows) {
      this.$confirm("确定要删除该工序吗？")
        .then(() => {
          rows.splice(index, 1);
          this.refreshSelect();
        })
        .catch(() => { });
    },
    handleDetailUpDown(id, index, rows, upDown, event) {
      if (event.target.nodeName === "I") {
        event.target.parentNode.blur();
      } else {
        event.target.blur();
      }
      if (
        (index == 0 && upDown == -1) ||
        (index == rows.length - 1 && upDown == 1)
      )
        return;
      var curRow = rows[index];
      var upDownRow = rows[index + upDown];
      // if (upDownRow.orderNo > this.dialogChildList.length) {
      //   return;
      // }
        //var curRowTmpOrderNo = curRow.orderNo;
        //curRow.orderNo = upDownRow.orderNo;
        //upDownRow.orderNo = curRowTmpOrderNo;
        //rows[index] = upDownRow;
        //rows[index + upDown] = curRow;

        var curRowstepId = curRow.stepId;
        curRow.stepId = upDownRow.stepId;
        upDownRow.stepId = curRowstepId;
    },
    ChangeSelect(val) {
      console.log(val);
      this.refreshSelect();
    },
    refreshSelect() {
      const _this = this;
      this.stepOptions.forEach(function (option) {
        option.hasSelect = false;
        _this.dialogChildList.forEach(function (value) {
          if (option.key == value.stepId) option.hasSelect = true;
        });
      });
    },
  },
};
</script>
