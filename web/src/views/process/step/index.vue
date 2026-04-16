<template>
  <div class="flex-column">
    <div class="filter-container">
      <el-card shadow="never" class="boby-small">
        <div slot="header" class="clearfix">
          <span>工序管理</span>
        </div>
        <div>
          <el-row :gutter="12">
            <el-col :span="16">
              <el-button type="primary" icon="el-icon-plus" size="small" @click="handleCreate">新增</el-button>
              <el-button type="primary" icon="el-icon-edit" size="small" @click="handleUpdate">编辑</el-button>
              <el-button type="danger" icon="el-icon-delete" size="small" @click="handleDelete">删除</el-button>
            </el-col>
            <el-col :span="8" style="text-align: right">
              <el-input
                v-model="listQuery.key"
                size="small"
                prefix-icon="el-icon-search"
                style="width: 240px"
                placeholder="关键字"
                @keyup.enter.native="handleFilter"
              />
              <el-button type="primary" size="small" icon="el-icon-search" @click="handleFilter">查询</el-button>
            </el-col>
          </el-row>
        </div>
      </el-card>
    </div>

    <div class="app-container fh">
      <el-table
        ref="stepTable"
        :data="list"
        v-loading="listLoading"
        row-key="id"
        style="width: 100%"
        :height="tableHeight"
        border
        fit
        stripe
        highlight-current-row
        @selection-change="handleSelectionChange"
        @row-click="rowClick"
        align="left"
      >
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column prop="code" label="工序编号" min-width="160px" sortable align="center" />
        <el-table-column prop="name" label="工序名称" min-width="200px" sortable align="center" />
        <el-table-column prop="stepType" label="类型" min-width="180px" :formatter="formatStepType" align="center" />
        <el-table-column prop="description" label="描述" min-width="240px" sortable align="center" />
      </el-table>

      <pagination
        v-show="total > 0"
        :total="total"
        :page.sync="listQuery.page"
        :limit.sync="listQuery.limit"
        @pagination="getList"
      />
    </div>

    <el-dialog v-el-drag-dialog :title="textMap[dialogStatus]" :visible.sync="dialogFormVisible" width="520px">
      <el-form ref="dataForm" :rules="rules" :model="temp" label-width="90px">
        <el-form-item label="编号" prop="code">
          <el-input v-model="temp.code" />
        </el-form-item>
        <el-form-item label="名称" prop="name">
          <el-input v-model="temp.name" />
        </el-form-item>
        <el-form-item label="类型" prop="stepType">
          <el-select v-model="temp.stepType" placeholder="请选择" style="width: 100%">
            <el-option v-for="item in typeOptions" :key="item.key" :label="item.display_name" :value="item.key" />
          </el-select>
        </el-form-item>
        <el-form-item label="描述" prop="description">
          <el-input v-model="temp.description" type="textarea" :autosize="{ minRows: 2, maxRows: 4 }" />
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormVisible = false">取消</el-button>
        <el-button type="primary" @click="dialogStatus === 'create' ? createData() : updateData()">确认</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import * as steps from "@/api/step";
import * as dictionaryDetails from "@/api/dictionaryDetail";
import waves from "@/directive/waves";
import Pagination from "@/components/Pagination";
import elDragDialog from "@/directive/el-dragDialog";

export default {
  name: "stepCrud",
  components: { Pagination },
  directives: {
    waves,
    elDragDialog,
  },
  data() {
    return {
      tableHeight: null,
      listLoading: false,
      list: [],
      total: 0,
      listQuery: {
        page: 1,
        limit: 20,
        key: undefined,
      },
      multipleSelection: [],
      dialogFormVisible: false,
      dialogStatus: "",
      textMap: {
        update: "编辑",
        create: "添加",
      },
      temp: {
        id: undefined,
        code: "",
        name: "",
        stepType: 2,
        description: "",
      },
      rules: {
        code: [{ required: true, message: "编号不能为空", trigger: "blur" }],
        name: [{ required: true, message: "名称不能为空", trigger: "blur" }],
        stepType: [{ required: true, message: "类型不能为空", trigger: "change" }],
      },
      typeOptions: [],
    };
  },
  mounted() {
    let h = document.documentElement.clientHeight;
    let topH = this.$refs.stepTable.$el.offsetTop;
    this.tableHeight = h - topH - 140;
    this.loadStepTypes();
  },
  methods: {
    rowClick(row) {
      this.$refs.stepTable.clearSelection();
      this.$refs.stepTable.toggleRowSelection(row);
    },
    handleSelectionChange(val) {
      this.multipleSelection = val;
    },
    formatStepType(row, column, cellValue) {
      const match = this.typeOptions.find((x) => x.key === cellValue);
      return match ? match.display_name : cellValue;
    },
    handleFilter() {
      this.listQuery.page = 1;
      this.getList();
    },
    resetTemp() {
      this.temp = {
        id: undefined,
        code: "",
        name: "",
        stepType: 2,
        description: "",
      };
    },
    handleCreate() {
      this.resetTemp();
      this.dialogStatus = "create";
      this.dialogFormVisible = true;
      this.$nextTick(() => {
        this.$refs["dataForm"].clearValidate();
      });
    },
    handleUpdate() {
      if (this.multipleSelection.length !== 1) {
        this.$message({
          message: "请选择一条数据进行编辑",
          type: "warning",
        });
        return;
      }
      this.temp = Object.assign({}, this.multipleSelection[0]);
      this.dialogStatus = "update";
      this.dialogFormVisible = true;
      this.$nextTick(() => {
        this.$refs["dataForm"].clearValidate();
      });
    },
    handleDelete() {
      if (this.multipleSelection.length <= 0) {
        this.$message({
          message: "请选择要删除的数据",
          type: "warning",
        });
        return;
      }
      const ids = this.multipleSelection.map((x) => x.id);
      const param = { ids };
      this.$confirm("确定要删除吗？")
        .then(() => {
          steps.del(param).then(() => {
            this.$notify({
              title: "成功",
              message: "删除成功",
              type: "success",
              duration: 2000,
            });
            this.getList();
          });
        })
        .catch(() => {});
    },
    createData() {
      this.$refs["dataForm"].validate((valid) => {
        if (!valid) return;
        const payload = Object.assign({}, this.temp, { steptype: this.temp.stepType });
        steps.add(payload).then(() => {
          this.dialogFormVisible = false;
          this.$notify({
            title: "成功",
            message: "创建成功",
            type: "success",
            duration: 2000,
          });
          this.getList();
        });
      });
    },
    updateData() {
      this.$refs["dataForm"].validate((valid) => {
        if (!valid) return;
        const tempData = Object.assign({}, this.temp, { steptype: this.temp.stepType });
        steps.update(tempData).then(() => {
          this.dialogFormVisible = false;
          this.$notify({
            title: "成功",
            message: "更新成功",
            type: "success",
            duration: 2000,
          });
          this.getList();
        });
      });
    },
    getList() {
      this.listLoading = true;
      steps
        .getList(this.listQuery)
        .then((res) => {
          const rows = res.data || res.result || [];
          this.list = rows.map((x) => {
            const stepType = x.stepType !== undefined ? x.stepType : x.steptype;
            return Object.assign({}, x, {
              stepType,
            });
          });
          this.total = res.count || 0;
        })
        .finally(() => {
          this.listLoading = false;
        });
    },
    loadStepTypes() {
      const fallback = [
        { key: 1, display_name: "自动站" },
        { key: 2, display_name: "线内人工站" },
        { key: 3, display_name: "线外人工站" },
        { key: 4, display_name: "线内可跳过人工站" },
        { key: 5, display_name: "模组数量监控站" },
      ];
      const param = {
        typeCode: "stepType",
      };
      dictionaryDetails.getListByType(param).then((response) => {
        const list = response.data || response.result || [];
        const dictOptions = list.map((item) => {
          return { key: item.value, display_name: item.name };
        });
        const merged = [];
        fallback.forEach((x) => {
          const dictItem = dictOptions.find((d) => d.key === x.key);
          merged.push(dictItem || x);
        });
        this.typeOptions = merged;
        this.getList();
      });
    },
  },
};
</script>

