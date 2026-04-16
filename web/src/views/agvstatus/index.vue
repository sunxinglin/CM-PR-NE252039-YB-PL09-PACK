<template>
  <!-- AGV状态监控 -->
  <div class="flex-column">
    <div class="filter-container">
      <el-card shadow="never" class="boby-small" style="height: 100%">
        <div slot="header" class="clearfix">
          <span>AGV状态监控</span>
        </div>
        <div>
          <el-row :gutter="2">
            <el-col :span="21">
              <el-button
                type="primary"
                icon="el-icon-plus"
                size="small"
                @click="handleCreate"
                >添加</el-button
              >
              <el-button
                type="primary"
                icon="el-icon-edit"
                size="small"
                @click="handleUpdate"
                >编辑</el-button
              >
              <el-button
                type="danger"
                icon="el-icon-delete"
                size="small"
                @click="handleDelete"
                >删除</el-button
              >
            </el-col>
            <el-col :span="3">
              <el-input
                @keyup.enter.native="handleFilter"
                prefix-icon="el-icon-search"
                size="small"
                style="width: 200px"
                class="filter-item"
                :placeholder="'AGV车号'"
                v-model="agvstatusListQuery.key"
                clearable
              ></el-input>
            </el-col>
          </el-row>
        </div>
      </el-card>
    </div>
    <div class="app-container flex-item">
      <div class="bg-white fh">
        <el-table
          ref="mainTable"
          :data="productList"
          v-loading="productListLoading"
          row-key="id"
          style="width: 100%"
          height="calc(100% - 52px)"
          @row-click="handleRowClick"
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
            prop="agvNo"
            label="AGV车号"
            min-width="20px"
            sortable
            align="center"
          >
          </el-table-column>
          <el-table-column
            prop="stationCode"
            label="工位编码"
            min-width="20px"
            sortable
            align="center"
          >
          </el-table-column>
          <el-table-column
            prop="packPN"
            label="PACK码"
            min-width="60px"
            sortable
            align="center"
          >
          </el-table-column>
          <el-table-column
            prop="holderBarCode"
            label="出货码"
            min-width="20px"
            sortable
            align="center"
          >
          </el-table-column>
          <el-table-column
            prop="remark"
            label="备注"
            min-width="20px"
            sortable
            align="center"
          >
          </el-table-column>
          <el-table-column label="操作" min-width="100px" align="center">
            <template slot-scope="scope">
              <el-button-group>
                <el-button
                  type="primary"
                  icon="el-icon-document-add"
                  size="small"
                  @click.native.prevent="handlebingagvpack(scope.row)"
                  >绑定</el-button
                >

                <el-button
                  style="margin-left: 20px"
                  type="danger"
                  icon="el-icon-document-remove"
                  size="small"
                  @click.native.prevent="handleunbingagv(scope.row)"
                  >解绑</el-button
                >
              </el-button-group>
            </template>
          </el-table-column>
        </el-table>
        <pagination
          :total="productTotal"
          v-show="productTotal > 0"
          :page.sync="agvstatusListQuery.page"
          :limit.sync="agvstatusListQuery.limit"
          @pagination="handleCurrentChange"
        />
      </div>

      <el-dialog
        v-el-drag-dialog
        class="dialog-mini"
        width="500px"
        :title="textMap[dialogStatus]"
        :visible.sync="dialogFormVisible"
      >
        <div>
          <el-form
            :rules="agvstatusRules"
            ref="dataForm"
            :model="agvstatusTemp"
            label-position="right"
            label-width="100px"
          >
            <el-form-item size="small" :label="'AGV车号'" prop="agvNo">
              <el-input
                v-model="agvstatusTemp.agvNo"
                :disabled="dialogStatus != 'create'"
                type="number"
                min="1"
              ></el-input>
            </el-form-item>
            <el-form-item size="small" :label="'备注'" prop="unitId">
              <el-input v-model="agvstatusTemp.remark"></el-input>
            </el-form-item>
          </el-form>
        </div>
        <div slot="footer">
          <el-button size="mini" @click="dialogFormVisible = false"
            >取消</el-button
          >
          <el-button
            size="mini"
            v-if="dialogStatus == 'create'"
            type="primary"
            @click="createData"
            >确认
          </el-button>
          <el-button size="mini" v-else type="primary" @click="updateData"
            >确认</el-button
          >
        </div>
      </el-dialog>

      <el-dialog
        v-el-drag-dialog
        class="dialog-mini"
        width="500px"
        title="AGV绑定PACK"
        :visible.sync="dialogbingagvpackVisible"
      >
        <div>
          <el-form
            ref="bindForm"
            :model="bingAGVPAck"
            label-position="right"
            label-width="100px"
            :rules="agvbingrule"
          >
            <el-form-item size="small" :label="'AGV车号'">
              <el-input v-model="agvstatusTemp.agvNo" disabled></el-input>
            </el-form-item>

            <el-form-item size="small" :label="'工位编码'" prop="stationcode">
              <el-select
                v-model="bingAGVPAck.stationcode"
                placeholder="请选择"
                filterable
                clearable
                :loading="stationOptionsLoading"
                style="width: 100%"
              >
                <el-option v-for="s in stationOptions" :key="s.code" :label="s.label" :value="s.code" />
              </el-select>
            </el-form-item>
            <el-form-item size="small" :label="'PACK码'" prop="packpn">
              <el-input v-model="bingAGVPAck.packpn"></el-input>
            </el-form-item>

            <el-form-item size="small" :label="'出货码'" prop="holderBarCode">
              <el-input v-model="bingAGVPAck.holderBarCode"></el-input>
            </el-form-item>
          </el-form>
        </div>
        <div slot="footer">
          <el-button size="mini" @click="dialogbingagvpackVisible = false"
            >取消</el-button
          >
          <el-button size="mini" type="primary" @click="bingagvpack"
            >确认</el-button
          >
        </div>
      </el-dialog>

    </div>
  </div>
</template>

<script>
import * as agvstatus from "@/api/agvstatus";
import * as stations from "@/api/station";
import * as dictionaryDetails from "@/api/dictionaryDetail";
import * as axios from "axios";
import waves from "@/directive/waves"; // 水波纹指令
import Sticky from "@/components/Sticky";
import permissionBtn from "@/components/PermissionBtn";
import Pagination from "@/components/Pagination";
import elDragDialog from "@/directive/el-dragDialog";

const validatePackPn = (rule, value, callback) => {
  if (value === undefined || value === null || value === "") {
    callback();
    return;
  }
  const len = String(value).length;
  if (len !== 12 && len !== 24) {
    callback(new Error("PACK码长度必须为12或24位"));
    return;
  }
  callback();
};

const validateHolderBarCode = (rule, value, callback) => {
  if (value === undefined || value === null || value === "") {
    callback();
    return;
  }
  const len = String(value).length;
  if (len !== 12) {
    callback(new Error("出货码长度必须为12位"));
    return;
  }
  callback();
};
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
      selectedAgvStatusRowId: null,
      productMultipleSelection: [], //勾选的数据表值
      productList: [], //数据表
      productTotal: 0, //数据条数
      productListLoading: true, //加载特效
      agvstatusListQuery: {
        //查询条件
        page: 1,
        limit: 20,
        key: undefined,
      },
      agvstatusTemp: {
        //模块临时值
        id: undefined,
        agvNo: "",
        remark: "",
        productType: "",
        packPN: "",
        stationCode: "",
        holderBarCode: "",
      },
      dialogFormVisible: false, //编辑框
      dialogbingagvpackVisible: false,
      dialogStatus: "", //编辑框功能(添加/编辑)
      textMap: {
        update: "编辑",
        create: "添加",
      },
      agvstatusRules: {
        //编辑框输入限制

        holderBarCode: [
          {
            required: true,
            message: "台车码不能为空",
            trigger: "blur",
          },
        ],
        agvNo: [
          {
            required: true,
            message: "AGV编号不能为空",
            trigger: "blur",
          },
        ],
      },
      agvbingrule: {
        stationcode: [
          {
            required: true,
            message: "工位编码不能为空",
            trigger: "change",
          },
        ],
        packpn: [
          {
            required: true,
            message: "pack条码不能为空",
            trigger: "blur",
          },
          {
            validator: validatePackPn,
            trigger: "blur",
          },
        ],
        holderBarCode: [
          {
            required: true,
            message: "出货码不能为空",
            trigger: "blur",
          },
          {
            validator: validateHolderBarCode,
            trigger: "blur",
          },
        ],
      },
      typeOptions: [],
      stationOptionsLoading: false,
      stationOptions: [],
      bingAGVPAck: {
        state: 0,
        agvcode: 0,
        stationcode: "",
        packpn: "",
        productType: "",
        holderBarCode: "",
      },
    };
  },
  mounted() {
    this.Load();
    this.loadStations();
  },
  methods: {
    //勾选框
    handleSelectionChange(val) {
      this.productMultipleSelection = val;
      if (val.length === 1) {
        this.selectedAgvStatusRowId = val[0].id;
      } else if (val.length === 0) {
        this.selectedAgvStatusRowId = null;
      } else {
        this.selectedAgvStatusRowId = null;
      }
    },
    handleRowClick(row, column) {
      if (column && (column.type === "selection" || column.label === "操作")) return;

      const table = this.$refs.mainTable;
      if (!table) return;

      const isSameRow = this.selectedAgvStatusRowId === row.id;
      table.clearSelection();
      if (isSameRow) {
        this.selectedAgvStatusRowId = null;
        return;
      }
      table.toggleRowSelection(row, true);
      this.selectedAgvStatusRowId = row.id;
    },
    //关键字搜索
    handleFilter() {
      this.Load();
    },
    //分页
    handleCurrentChange(val) {
      this.agvstatusListQuery.page = val.page;
      this.agvstatusListQuery.limit = val.limit;
      this.Load(); //页面加载
    },
    //列表加载
    Load() {
      if (
        this.agvstatusListQuery.key != undefined &&
        typeof this.agvstatusListQuery.key != "number" &&
        this.agvstatusListQuery.key % 1 != 0
      ) {
        this.$message({
          message: "搜索关键字为AGV车号，车号为数字",
          type: "error",
        });
        return;
      }
      this.productListLoading = true;
      agvstatus.load(this.agvstatusListQuery).then((response) => {
        this.productList = response.data; //提取数据表
        this.productTotal = response.count; //提取数据表条数
        this.productListLoading = false;
      });
    },

    //编辑框数值初始值
    resetTemp() {
      this.agvstatusTemp = {
        id: undefined,
        agvNo: "",
        remark: "",
        productType: "",
        packPN: "",
        stationCode: "",
        holderBarCode: "",
      };
    },
    resetbingagvpack() {
      this.bingAGVPAck = {
        state: 0,
        agvcode: 0,
        packpn: "",
        productType: "",
        stationcode: "",
        holderBarCode: "",
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
    //保存提交
    createData() {
      this.$refs["dataForm"].validate((valid) => {
        if (valid) {
          agvstatus.add(this.agvstatusTemp).then((response) => {
            this.dialogFormVisible = false; //编辑框关闭
            this.$notify({
              title: "成功",
              message: "创建成功",
              type: "success",
              duration: 2000,
            });
            this.Load(); //页面加载
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
        //弹出编辑框
        this.agvstatusTemp = Object.assign({}, row); //复制选中的数据
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
          agvstatus.update(this.agvstatusTemp).then(() => {
            this.dialogFormVisible = false; //编辑框关闭
            this.$notify({
              title: "成功",
              message: "更新成功",
              type: "success",
              duration: 2000,
            });
            this.Load(); //页面加载
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
          agvstatus
            .del(this.productMultipleSelection.map((u) => u.id))
            .then(() => {
              this.$notify({
                title: "成功",
                message: "删除成功",
                type: "success",
                duration: 2000,
              });
              this.Load(); //页面加载
            });
        })
        .catch((_) => {});
    },
    handlebingagvpack(row) {
      this.bingAGVPAck.state = 1;
      this.bingAGVPAck.agvcode = row.agvNo;
      this.bingAGVPAck.stationcode = row.stationCode || "";
      this.bingAGVPAck.packpn = row.packPN || "";
      this.bingAGVPAck.holderBarCode = row.holderBarCode || "";
      this.dialogbingagvpackVisible = true;
      // this.bingAGVPAck.agvtemp = Object.assign({}, row); //复制选中的数据
      this.agvstatusTemp = Object.assign({}, row);
    },
    handleunbingagv(row) {
      this.bingAGVPAck.state = 2;
      this.bingAGVPAck.agvcode = row.agvNo;
      this.bingAGVPAck.packpn = row.packPN;
      this.bingAGVPAck.productType = row.productType;
      this.bingagvpack();
      // this.bingAGVPAck.agvtemp = Object.assign({}, row); //复制选中的数据
    },
    loadStations() {
      this.stationOptionsLoading = true;
      return stations
        .GetAllStation()
        .then((res) => {
          const rows = res.result || res.data || [];
          this.stationOptions = rows.map((x) => {
            const code = x.code || "";
            const name = x.name || "";
            const label = code && name ? `${code} - ${name}` : code || name;
            return { code, name, label };
          });
        })
        .finally(() => {
          this.stationOptionsLoading = false;
        });
    },
    bingagvpack() {
      const run = () => {
        agvstatus.BingAgv(this.bingAGVPAck).then((_) => {
        if (this.bingAGVPAck.state == 1) {
          this.$notify({
            title: "成功",
            message: "绑定成功",
            type: "success",
            duration: 2000,
          });
        } else {
          this.$notify({
            title: "成功",
            message: "解绑成功",
            type: "success",
            duration: 2000,
          });
        }
        this.dialogbingagvpackVisible = false;
        this.resetbingagvpack();
        this.Load();
      });
      };

      if (this.bingAGVPAck.state == 2) {
        run();
        return;
      }

      this.$refs["bindForm"].validate((valid) => {
        if (!valid) return;
        run();
      });
    },
  },
};
</script>
