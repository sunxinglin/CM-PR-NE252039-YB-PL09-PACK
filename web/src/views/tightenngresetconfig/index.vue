<template>
  <div class="flex-column">
    <div class="filter-container">
      <el-card shadow="never" class="boby-small" style="height: 100%">
        <div slot="header" class="clearfix">
          <span>拧紧NG管控</span>
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
          </el-row>
        </div>
      </el-card>
    </div>
    <div class="app-container fh">
      <el-table ref="mainTable" :data="screwNGResetConfigList" v-loading="screwNGResetConfigListLoading" row-key="id"
        style="width: 100%" height="calc(100% - 52px)" @row-click="handleRowClick"
        @selection-change="handleSelectionChange" border fit stripe highlight-current-row align="left">
        <el-table-column type="selection" align="center" width="55"></el-table-column>
        <el-table-column prop="step.code" label="工序" min-width="20px" sortable align="center"></el-table-column>
        <el-table-column prop="singleScrewResetNum" label="拧紧NG复位次数（单颗）" min-width="20px" sortable
          align="center"></el-table-column>
        <el-table-column prop="roleNameArray" label="权限组" min-width="20px" sortable align="center"
          :formatter="formatterRoleNameArray"></el-table-column>
      </el-table>

      <pagination :total="screwNGResetConfigTotal" v-show="screwNGResetConfigTotal > 0"
        :page.sync="screwNGResetConfigListQuery.page" :limit.sync="screwNGResetConfigListQuery.limit"
        @pagination="handleCurrentChange" />
    </div>

    <el-dialog class="dialog-mini" width="600px" :title="textMap[dialogStatus]" :close-on-click-modal=false
      :visible.sync="dialogFormVisible">
      <div>
        <el-form :rules="stepRules" ref="dataForm" :model="TempData" label-position="top" size="small">
          <el-form-item :label="'工序'">
            <el-select class="filter-item" style="width: 100%" learable filterable remote reserve-keyword
              v-model="TempData.stepId" placeholder="请选择工序">
              <el-option v-for="item in stepOptions" :key="item.key" :label="item.display_name"
                :value="item.key"></el-option>
            </el-select>
          </el-form-item>

          <el-form-item size="small" :label="'拧紧NG复位次数（单颗）'" prop="singleScrewResetNum">
            <el-input v-model="TempData.singleScrewResetNum"></el-input>
          </el-form-item>

          <el-form-item :label="'权限'">
            <el-select class="filter-item" style="width: 100%" multiple placeholder="请选择权限"
              v-model="TempData.roleIdArray">
              <el-option v-for="item in roleOptions" :key="item.key" :label="item.display_name"
                :value="item.key"></el-option>
            </el-select>
          </el-form-item>

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
import * as screwNGResetConfigApi from "@/api/screwNGResetConfig";
import * as steps from "@/api/step";
import * as roleApi from "@/api/roles";
import waves from "@/directive/waves"; // 水波纹指令
import Sticky from "@/components/Sticky";
import permissionBtn from "@/components/PermissionBtn";
import Pagination from "@/components/Pagination";
import elDragDialog from "@/directive/el-dragDialog";
export default {
  name: "ScrewNGResetConfig",

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
      display_name: "",
      selectedScrewNGResetConfigRowId: null,
      stepMultipleSelection: [], //勾选的数据表值
      screwNGResetConfigList: [], //数据表
      screwNGResetConfigTotal: 0, //数据条数
      screwNGResetConfigListLoading: true, //加载特效
      screwNGResetConfigListQuery: {
        //查询条件
        page: 1,
        limit: 20,
        key: undefined,
      },
      stepOptions: [],
      roleOptions: [],
      TempData: {
        //模块临时值
        id: undefined,
        stepId: undefined,
        singleScrewResetNum: 0,
        roleIdArray: [],
      },
      dialogFormVisible: false, //编辑框
      dialogStatus: "", //编辑框功能(添加/编辑)
      textMap: { update: "编辑", create: "添加", },
      stepRules: {
        //编辑框输入限制
        stationCode: [{ required: true, message: "工位不能为空", trigger: "blur", },],
        name: [{ required: true, message: "名称不能为空", trigger: "blur", },],
        deviceNo: [{ required: true, message: "设备号不能为空", trigger: "blur", },],
      },
    };
  },
  mounted() {
    this.Load();
    this.getStepList();
    this.getRoleList();
  },
  methods: {
    //勾选框
    handleSelectionChange(val) {
      this.stepMultipleSelection = val;
      if (val.length === 1) {
        this.selectedScrewNGResetConfigRowId = val[0].id;
      } else if (val.length === 0) {
        this.selectedScrewNGResetConfigRowId = null;
      } else {
        this.selectedScrewNGResetConfigRowId = null;
      }
    },
    handleRowClick(row, column) {
      if (column && column.type === "selection") return;

      const table = this.$refs.mainTable;
      if (!table) return;

      const isSameRow = this.selectedScrewNGResetConfigRowId === row.id;
      table.clearSelection();
      if (isSameRow) {
        this.selectedScrewNGResetConfigRowId = null;
        return;
      }
      table.toggleRowSelection(row, true);
      this.selectedScrewNGResetConfigRowId = row.id;
    },
    //关键字搜索
    handleFilter() {
      this.Load();
    },

    //分页
    handleCurrentChange(val) {
      this.screwNGResetConfigListQuery.page = val.page;
      this.screwNGResetConfigListQuery.limit = val.limit;
      this.Load(); //页面加载
    },

    //列表加载
    Load() {
      this.screwNGResetConfigListLoading = true;
      screwNGResetConfigApi.getPageList(this.screwNGResetConfigListQuery).then((response) => {
        this.screwNGResetConfigList = response.data; //提取数据表
        this.screwNGResetConfigTotal = response.count; //提取数据表条数
        this.screwNGResetConfigListLoading = false;
      });
    },
    //编辑框数值初始值
    resetTemp() {
      this.TempData = {
        id: undefined,
        stationCode: "",
        deviceNo: "1",
        name: "",
        baud: "115200",
        port: "",
        ipAddress: "192.168.1.30",
        protocolType: 1,
        description: "",
        deviceBrand: 1,
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
          screwNGResetConfigApi.add(this.TempData).then((response) => {
            this.dialogFormVisible = false; //编辑框关闭
            this.$notify({
              title: "成功",
              message: "创建成功",
              type: "success",
              duration: 200,
            });
            this.Load(); //页面加载
          });
        }
      });
    },
    //点击编辑
    handleUpdate() {
      if (this.stepMultipleSelection.length !== 1) {
        this.$message({
          message: "只能选中一个进行编辑",
          type: "error",
        });
        return;
      } else {
        var row = this.stepMultipleSelection[0];
        //弹出编辑框
        this.TempData = Object.assign({}, row); //复制选中的数据

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
          screwNGResetConfigApi.update(this.TempData).then((response) => {
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
      if (this.stepMultipleSelection.length < 1) {
        this.$message({
          message: "至少删除一个",
          type: "error",
        });
        return;
      }
      this.$confirm("确定要删除吗？")
        .then((_) => {
          var rows = this.stepMultipleSelection;
          var selectids = rows.map((u) => u.id); //提取复选框的数据的Id
          var param = {
            ids: selectids,
          };
          screwNGResetConfigApi.del(param).then(() => {
            this.$notify({
              title: "成功",
              message: "删除成功",
              type: "success",
              duration: 2000,
            });
            this.Load(); //页面加载
          });
        })
        .catch((_) => { });
    },

    getStepList() {
      var _this = this; // 记录vuecomponent
      var param = { "key": null };
      steps.getList(param).then((response) => {
        _this.steps = response.data.map(function (item) {
          return {
            key: item.id,
            display_name: item.code,
            steptype: item.steptype,
          };
        });
        _this.stepOptions = JSON.parse(JSON.stringify(_this.steps));

      });
    },

    getRoleList() {
      var _this = this; // 记录vuecomponent
      var param = { "key": null };
      roleApi.getList(param).then((response) => {
        const s = response.result.map(function (item) {
          return {
            key: item.id,
            display_name: item.claimValue,
          };
        });
        _this.roleOptions = JSON.parse(JSON.stringify(s));

      });
    },
    formatterRoleNameArray(row, column, cellValue) {
      return cellValue.join(", ");
    },
  },
};
</script>
