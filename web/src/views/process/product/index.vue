<template>
  <div class="flex-column">
    <div class="filter-container">
      <el-card shadow="never" class="boby-small" style="height: 100%">
        <div slot="header" class="clearfix">
          <span>产品表</span>
        </div>
        <div>
          <el-row :gutter="2">
            <el-col :span="21">
              <el-button type="primary" icon="el-icon-plus" size="small" @click="handleCreate">添加
              </el-button>
              <el-button type="primary" icon="el-icon-edit" size="small" @click="handleUpdate">编辑
              </el-button>
              <el-button type="danger" icon="el-icon-delete" size="small" @click="handleDelete">删除
              </el-button>
              <!-- <el-button type="primary" icon="el-icon-delete" size="small" @click="domainvisible">模组条码规则
              </el-button> -->
            </el-col>
            <el-col :span="3">
              <el-input @keyup.enter.native="handleFilter" prefix-icon="el-icon-search" size="small"
                style="width: 200px" class="filter-item" :placeholder="'PN号/名称'" v-model="productListQuery.key"
                clearable></el-input>
            </el-col>
          </el-row>
        </div>
      </el-card>
    </div>

    <div class="app-container fh">
      <el-table ref="mainTable" :data="productList" v-loading="productListLoading" row-key="id" style="width: 100%"
        height="calc(100% - 52px)" @row-click="handleRowClick" @selection-change="handleSelectionChange" border fit
        stripe highlight-current-row
        align="left">
        <el-table-column type="selection" min-width="20px" align="center"></el-table-column>
        <el-table-column prop="code" label="PN号"  sortable align="center"></el-table-column>
        <el-table-column prop="name" label="名称"  sortable align="center"></el-table-column>
        <el-table-column prop="specification" label="产品描述"  sortable align="center">
        </el-table-column>
      </el-table>
      <pagination :total="productTotal" v-show="productTotal > 0" :page.sync="productListQuery.page"
        :limit.sync="productListQuery.limit" @pagination="handleCurrentChange" />
    </div>

    <el-dialog v-el-drag-dialog class="dialog-mini" width="500px" :title="textMap[dialogStatus]"
      :visible.sync="dialogFormVisible">
      <div>
        <el-form :rules="productRules" ref="dataForm" :model="productTemp" label-position="right" label-width="100px">
          <el-form-item size="small" :label="'PN号'" prop="code">
            <el-input v-model="productTemp.code"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'名称'" prop="name">
            <el-input v-model="productTemp.name"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'描述'">
            <el-input v-model="productTemp.specification"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'产品类型'" prop="typeid">
            <el-select v-model="productTemp.typeId" placeholder="请选择">
              <el-option v-for="item in typeOptions" :key="item.key" :label="item.display_name" :value="item.key">
              </el-option>
            </el-select>
          </el-form-item>
          <el-form-item size="small" :label="'Pack条码规则'" prop="packPNRule">
            <el-input v-model="productTemp.packPNRule"></el-input>
          </el-form-item>
          <!-- <el-form-item size="small" :label="'Pack出货码规则'" prop="packPNRule">
            <el-input v-model="productTemp.packOutCodeRule"></el-input>
          </el-form-item> -->


        </el-form>
      </div>
      <div slot="footer">
        <el-button size="mini" @click="dialogFormVisible = false">取消</el-button>
        <el-button size="mini" v-if="dialogStatus == 'create'" type="primary" @click="createData">确认</el-button>
        <el-button size="mini" v-else type="primary" @click="updateData">确认</el-button>
      </div>
    </el-dialog>
    <el-dialog v-el-drag-dialog class="dialog-mini" width="500px" :visible.sync="dialogdomainvisible">
      <div>
        <el-table ref="tableData" :data="domainlist" row-key="id" style="width: 100%" height="600px" border fit stripe
          highlight-current-row align="left">
          <el-table-column label="模组条码规则" min-width="100px" sortable align="center">
            <template slot-scope="scope">
              <el-input v-model="scope.row.value" />
            </template>
          </el-table-column>
          <el-table-column prop="sequence" label="操作" min-width="60px" sortable align="center">
            <template slot-scope="scope">
              <el-button size="small" plain type="danger" icon="el-icon-delete"
                @click.native.prevent="removeDomain(scope.row)"></el-button>
            </template>
          </el-table-column>
        </el-table>
      </div>
      <div slot="footer">
        <el-button size="mini" @click="dialogdomainvisible = false">取消</el-button>
        <el-button @click="addDomain" size="mini">新增模组条码规则</el-button>
        <el-button size="mini" type="primary" @click="DomainUpdate">确认</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import * as products from "@/api/product";
import * as dictionaryDetails from "@/api/dictionaryDetail";
import * as axios from "axios";
import waves from "@/directive/waves"; // 水波纹指令
import Sticky from "@/components/Sticky";
import permissionBtn from "@/components/PermissionBtn";
import Pagination from "@/components/Pagination";
import elDragDialog from "@/directive/el-dragDialog";
export default {
  name: "product",

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
      selectedProductRowId: null,
      productMultipleSelection: [], //勾选的数据表值
      productList: [], //数据表
      domainlist: [],
      productTotal: 0, //数据条数
      productListLoading: true, //加载特效
      productListQuery: {
        //查询条件
        page: 1,
        limit: 20,
        key: undefined,
      },
      productTemp: {
        //模块临时值
        id: undefined,
        code: "",
        name: "",
        typeId: 0,
        specification: "",

        packPNRule: "",
        packOutCodeRule: "",
        modelRules: [
          {
            value: "",
          },
        ],
      },
      dialogFormVisible: false, //编辑框
      dialogdomainvisible: false,
      dialogStatus: "", //编辑框功能(添加/编辑)
      textMap: {
        update: "编辑",
        create: "添加",
      },
      productRules: {
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
        packPNRule: [
          {
            required: true,
            message: "PACK条码规则不可为空",
            trigger: "blur",
          },
        ],
      },
      typeOptions: [],
    };
  },
  mounted() {
    this.productLoad();
    this.getProductType();
  },
  methods: {
    //勾选框
    handleSelectionChange(val) {
      this.productMultipleSelection = val;
      if (val.length === 1) {
        this.selectedProductRowId = val[0].id;
      } else if (val.length === 0) {
        this.selectedProductRowId = null;
      } else {
        this.selectedProductRowId = null;
      }
    },
    handleRowClick(row, column) {
      if (column && column.type === "selection") return;

      const table = this.$refs.mainTable;
      if (!table) return;

      const isSameRow = this.selectedProductRowId === row.id;
      table.clearSelection();
      if (isSameRow) {
        this.selectedProductRowId = null;
        return;
      }
      table.toggleRowSelection(row, true);
      this.selectedProductRowId = row.id;
    },
    //关键字搜索
    handleFilter() {
      this.productLoad();
    },
    //分页
    handleCurrentChange(val) {
      this.productListQuery.page = val.page;
      this.productListQuery.limit = val.limit;
      this.productLoad(); //页面加载
    },
    //列表加载
    productLoad() {
      this.productListLoading = true;
      products.load(this.productListQuery).then((response) => {
        this.productList = response.data; //提取数据表
        this.productTotal = response.count; //提取数据表条数
        this.productListLoading = false;
      });
    },
    getProductType() {
      var _this = this; // 记录vuecomponent
      var param = {
        typeCode: "ProductType",
      };
      dictionaryDetails.getListByType(param).then((response) => {
        _this.types = response.data.map(function (item) {
          return {
            key: item.id,
            display_name: item.name,
          };
        });
        _this.typeOptions = JSON.parse(JSON.stringify(_this.types));
      });
    },
    //编辑框数值初始值
    resetTemp() {
      this.productTemp = {
        id: undefined,
        code: "",
        name: "",
        typeId: 0,
        specification: "",
        packPNRule: "",
        modelRulesstr: ""
      };
      console.log(this.productTemp.modelRules);
      if (this.typeOptions.length > 0 && this.productTemp.typeId == 0) {
        this.productTemp.typeId = this.typeOptions[0].key;

      }
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
    //保存提交
    createData() {
      this.$refs["dataForm"].validate((valid) => {
        if (valid) {
          products.add(this.productTemp).then((response) => {
            this.dialogFormVisible = false; //编辑框关闭
            this.$notify({
              title: "成功",
              message: "创建成功",
              type: "success",
              duration: 2000,
            });
            this.productLoad(); //页面加载
          });
        }
      });
    },
    //点击编辑
    handleUpdate() {
      if (this.productMultipleSelection.length !== 1) {
        this.$message({
          message: "只能选中一个进行编辑",
          type: "error",
        });
        return;
      } else {
        var row = this.productMultipleSelection[0];
        console.log(row);
        //弹出编辑框
        this.productTemp = Object.assign({}, row); //复制选中的数据
        this.dialogStatus = "update"; //编辑框功能选择（更新）
        this.dialogFormVisible = true; //编辑框显示
        this.$nextTick(() => {
          this.$refs["dataForm"].clearValidate();
        });
      }
    },
    //更新提交
    updateData() {
      this.$refs["dataForm"].validate((valid) => {
        if (valid) {
          products.update(this.productTemp).then(() => {
            this.dialogFormVisible = false; //编辑框关闭
            this.$notify({
              title: "成功",
              message: "更新成功",
              type: "success",
              duration: 2000,
            });
            this.productLoad(); //页面加载
          });
        }
      });
    },
    //点击删除
    handleDelete(row) {
      if (this.productMultipleSelection.length < 1) {
        this.$message({
          message: "至少删除一个",
          type: "error",
        });
        return;
      }
      this.$confirm("确定要删除吗？")
        .then((_) => {
          var rows = this.productMultipleSelection;
          var selectids = rows.map((u) => u.id); //提取复选框的数据的Id
          var param = {
            ids: selectids,
          };
          products.del(param).then(() => {
            this.$notify({
              title: "成功",
              message: "删除成功",
              type: "success",
              duration: 2000,
            });
            this.productLoad(); //页面加载
          });
        })
        .catch((_) => { });
    },
    removeDomain(item) {
      var index = this.productTemp.modelRules.indexOf(item);
      if (index !== -1) {
        this.productTemp.modelRules.splice(index, 1);
      }
      this.productTemp.modelRulesstr = JSON.stringify(this.productTemp.modelRules);
      console.log(this.productTemp.modelRules);
    },
    addDomain() {
      this.domainlist.push({
        value: "",
        key: Date.now(),
      });
      this.$nextTick(() => {
        this.$refs.tableData.bodyWrapper.scrollTop = this.$refs.tableData.bodyWrapper.scrollHeight
      })
    },
    domainvisible() {
      if (this.productMultipleSelection.length !== 1) {
        this.$message({
          message: "只能选中一个进行编辑",
          type: "error",
        });
        return;
      } else {
        var row = this.productMultipleSelection[0];

        //弹出编辑框
        this.productTemp = Object.assign({}, row); //复制选中的数据
        this.domainlist = this.productTemp.modelRules;
        this.dialogStatus = "update"; //编辑框功能选择（更新）
        this.dialogdomainvisible = true;

      }


    },
    DomainUpdate() {
      this.productTemp.modelRules = this.domainlist;
      this.productTemp.modelRulesstr = ""
      console.log(this.productTemp);
      products.update(this.productTemp).then(() => {
        this.dialogdomainvisible = false; //编辑框关闭
        this.$notify({
          title: "成功",
          message: "更新成功",
          type: "success",
          duration: 2000,
        });
        this.productLoad(); //页面加载
      });
    }
  },
};
</script>
