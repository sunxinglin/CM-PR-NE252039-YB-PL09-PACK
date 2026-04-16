<template>
  <div class="flex-column">
    <div class="filter-container">
      <el-card shadow="never" class="boby-small" style="height: 100%">
        <div slot="header" class="clearfix">
          <span>新自动拧紧数据查询</span>
        </div>
        <div>
          <el-row :gutter="2">
            <el-col :span="21">
              <el-input prefix-icon="el-icon-search" size="small" style="width: 240px" class="filter-item"
                :placeholder="'PACK码'" v-model="query.PackCode" clearable>
              </el-input>

              <el-date-picker clearable filterable size="small" class="filter-item" placeholder="开始时间" type="datetime"
                :editable="false" v-model="query.BeginTime" value-format="yyyy-MM-dd HH:mm:ss" @change="changeDate"
                :picker-options="pickerOptions0">
              </el-date-picker>

              <el-date-picker clearable filterable size="small" class="filter-item" placeholder="结束时间" type="datetime"
                :editable="false" v-model="query.EndTime" value-format="yyyy-MM-dd HH:mm:ss" @change="changeDate"
                :picker-options="pickerOptions1">
              </el-date-picker>

              <el-select class="filter-item" size="small" style="width: 200px" v-model="query.TightenType"
                placeholder="拧紧类型" clearable>
                <el-option v-for="item in tightenTypeOptions" :key="item.value" :label="item.label"
                  :value="item.value"></el-option>
              </el-select>

              <el-button icon="el-icon-search" size="small" @click="handleFilter" style="margin-left: 10px">
                查询
              </el-button>

              <json-excel class="filter-item" :fetch="fetchExportData" :fields="excelFields" :name="excelName">
                <el-button type="primary" icon="el-icon-download" size="small" :loading="exportLoading">
                  导出
                </el-button>
              </json-excel>
            </el-col>
          </el-row>
        </div>
      </el-card>
    </div>

    <div class="app-container fh">
      <el-table ref="mainTable" :data="list" v-loading="listLoading" row-key="id" style="width: 100%"
        :height="tableHeight" @row-click="handleRowClick" @selection-change="handleSelectionChange" border fit stripe
        highlight-current-row align="left">
        <el-table-column type="selection" align="center" width="55"></el-table-column>
        <el-table-column prop="id" label="ID" min-width="75px" sortable align="center"></el-table-column>
        <el-table-column prop="sfc" label="Pack码" min-width="175px" sortable align="center"></el-table-column>
        <el-table-column prop="stationName" label="工位" min-width="140px" sortable align="center"></el-table-column>
        <el-table-column prop="tightenType" label="拧紧类型" min-width="80px" sortable align="center"
          :formatter="formatTightenType"></el-table-column>
        <el-table-column prop="createTime" label="创建时间" min-width="120px" sortable align="center"></el-table-column>
        <el-table-column prop="updateTime" label="更新时间" min-width="120px" sortable align="center"></el-table-column>
        <el-table-column prop="totalCount" label="总数" min-width="75px" sortable align="center"></el-table-column>
        <el-table-column prop="okCount" label="OK数" min-width="75px" sortable align="center"></el-table-column>
        <el-table-column prop="ngCount" label="NG数" min-width="75px" sortable align="center"></el-table-column>
        <el-table-column label="操作" min-width="170px" align="center" class-name="operation-column">
          <template slot-scope="scope">
            <el-button size="mini" type="success" @click.native.prevent="openDetail(scope.row)">查看详情</el-button>
            <el-button size="mini" type="primary" :loading="exportDetailLoadingId === scope.row.id"
              @click.native.prevent="exportDetail(scope.row)">导出明细</el-button>
          </template>
        </el-table-column>
      </el-table>

      <pagination :total="total" v-show="total > 0" :page.sync="query.Page" :limit.sync="query.Limit"
        @pagination="handleCurrentChange" />
    </div>

    <el-dialog v-el-drag-dialog class="dialog-mini" width="900px" :visible.sync="detailVisible"
      title="自动拧紧明细">
      <div>
        <el-table ref="detailTable" :data="detailList" v-loading="detailLoading" row-key="index" style="width: 100%"
          :height="detailTableHeight" border fit stripe highlight-current-row align="left">
          <el-table-column prop="orderNo" label="序号" min-width="80px" sortable align="center"></el-table-column>
          <el-table-column prop="programNo" label="程序号" min-width="90px" sortable align="center"></el-table-column>
          <el-table-column prop="resultOK" label="结果" min-width="75px" sortable align="center"
            :formatter="formatResultOK"></el-table-column>
          <el-table-column prop="torqueResult.tagValue" label="扭力" min-width="120px" sortable
            align="center"></el-table-column>
          <el-table-column prop="angleResult.tagValue" label="角度" min-width="120px" sortable
            align="center"></el-table-column>
          <el-table-column prop="torqueResult.mesName" label="扭力上传代码" min-width="160px" sortable
            align="center"></el-table-column>
          <el-table-column prop="angleResult.mesName" label="角度上传代码" min-width="160px" sortable
            align="center"></el-table-column>
        </el-table>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import waves from "@/directive/waves";
import Pagination from "@/components/Pagination";
import elDragDialog from "@/directive/el-dragDialog";
import JsonExcel from "vue-json-excel";
import { formatTime } from "@/utils";
import * as api from "@/api/autoTightenExternal";

export default {
  name: "ExternalAutoTightenData",
  components: {
    Pagination,
    JsonExcel,
  },
  directives: {
    waves,
    elDragDialog,
  },
  data() {
    return {
      selectedRowId: null,
      list: [],
      total: 0,
      listLoading: false,
      exportLoading: false,
      exportDetailLoadingId: null,
      tableHeight: null,
      pickerOptions0: {},
      pickerOptions1: {},
      query: {
        Page: 1,
        Limit: 20,
        PackCode: "",
        BeginTime: "",
        EndTime: "",
        TightenType: undefined,
      },
      tightenTypeOptions: [
        { value: 0, label: "模组" },
        { value: 1, label: "上盖" },
        { value: 2, label: "压条" },
      ],
      detailVisible: false,
      detailLoading: false,
      detailList: [],
      detailTableHeight: 520,
      excelFields: {
        ID: "id",
        SFC: "sfc",
        工位: "stationName",
        拧紧类型: "tightenTypeName",
        创建时间: "createTime",
        更新时间: "updateTime",
        总数: "totalCount",
        OK数: "okCount",
        NG数: "ngCount",
      },
    };
  },
  computed: {
    excelName() {
      return `新自动拧紧数据_${formatTime("yyyyMMddhhmmss")}.xls`;
    },
  },
  mounted() {
    this.$nextTick(() => {
      const table = this.$refs.mainTable;
      if (!table || !table.$el) return;
      const h = document.documentElement.clientHeight;
      const topH = table.$el.offsetTop;
      const tableHeight = Math.floor((h - topH) * 0.78);
      this.tableHeight = tableHeight >= 240 ? tableHeight : 240;
    });

    this.changeDate();
  },
  methods: {
    getTightenTypeLabel(value) {
      const t = value === null || value === undefined ? null : Number(value);
      const hit = this.tightenTypeOptions.find((o) => o.value === t);
      return hit ? hit.label : value;
    },
    formatTightenType(row, column, cellValue) {
      return this.getTightenTypeLabel(cellValue);
    },
    changeDate() {
      const date1 = new Date(this.query.BeginTime).getTime();
      const date2 = new Date(this.query.EndTime).getTime();
      this.pickerOptions0 = {
        disabledDate: (time) => {
          if (date2) return time.getTime() >= date2;
        },
      };
      this.pickerOptions1 = {
        disabledDate: (time) => {
          if (date1) return time.getTime() <= date1;
        },
      };
    },
    normalizeListResponse(response) {
      const rows = response && (response.result || response.data || []);
      const count = response && (response.count || response.total || response.Count || 0);
      return { rows, count };
    },
    handleCurrentChange(val) {
      this.query.Page = val.page;
      this.query.Limit = val.limit;
      this.getList();
    },
    handleFilter() {
      this.query.Page = 1;
      this.getList();
    },
    getList() {
      this.listLoading = true;
      const params = Object.assign({}, this.query);
      if (params.TightenType === null || params.TightenType === "" || params.TightenType === undefined) {
        delete params.TightenType;
      }
      if (!params.BeginTime) delete params.BeginTime;
      if (!params.EndTime) delete params.EndTime;
      if (!params.PackCode) delete params.PackCode;

      api
        .loadExternalAutoTightenData(params)
        .then((response) => {
          const { rows, count } = this.normalizeListResponse(response);
          this.list = (rows || []).map((r) => Object.assign({}, r, { tightenTypeName: this.getTightenTypeLabel(r.tightenType) }));
          this.total = count;
        })
        .finally(() => {
          this.listLoading = false;
        });
    },
    handleSelectionChange(val) {
      if (val.length === 1) {
        this.selectedRowId = val[0].id;
      } else if (val.length === 0) {
        this.selectedRowId = null;
      } else {
        this.selectedRowId = null;
      }
    },
    handleRowClick(row, column) {
      if (column && (column.type === "selection" || column.label === "操作")) return;

      const table = this.$refs.mainTable;
      if (!table) return;

      const isSameRow = this.selectedRowId === row.id;
      table.clearSelection();
      if (isSameRow) {
        this.selectedRowId = null;
        return;
      }
      table.toggleRowSelection(row, true);
      this.selectedRowId = row.id;
    },
    openDetail(row) {
      this.detailVisible = true;
      this.detailLoading = true;
      this.detailList = [];

      api
        .loadExternalAutoTightenDataDetail({ dataId: row.id })
        .then((response) => {
          this.detailList = (response && (response.result || response.data || [])) || [];
        })
        .finally(() => {
          this.detailLoading = false;
        });

      this.$nextTick(() => {
        const table = this.$refs.detailTable;
        if (!table || !table.$el) return;
        const h = document.documentElement.clientHeight;
        const topH = table.$el.offsetTop;
        const tableHeight = Math.floor((h - topH) * 0.65);
        this.detailTableHeight = tableHeight >= 240 ? tableHeight : 240;
      });
    },
    formatResultOK(row, column, cellValue) {
      if (cellValue === 1 || cellValue === true) return "OK";
      if (cellValue === 0 || cellValue === false) return "NG";
      return cellValue;
    },
    downloadBlob(blob, fileName) {
      const b = blob instanceof Blob ? blob : new Blob([blob]);
      const name = fileName || "download.xlsx";
      if (window.navigator.msSaveOrOpenBlob) {
        navigator.msSaveBlob(b, name);
        return;
      }
      const link = document.createElement("a");
      link.href = window.URL.createObjectURL(b);
      link.download = name;
      link.click();
      window.URL.revokeObjectURL(link.href);
    },
    exportDetail(row) {
      this.exportDetailLoadingId = row.id;
      api
        .exportExternalAutoTightenDataDetail({ id: row.id })
        .then((data) => {
          const pack = row.sfc || row.packCode || row.id;
          this.downloadBlob(data, `自动拧紧明细_${pack}.xlsx`);
        })
        .finally(() => {
          this.exportDetailLoadingId = null;
        });
    },
    async fetchExportData() {
      this.exportLoading = true;
      try {
        const maxExport = 20000;
        const pageSize = 2000;

        const baseParams = Object.assign({}, this.query, {
          Page: 1,
          Limit: pageSize,
        });
        if (baseParams.TightenType === null || baseParams.TightenType === "" || baseParams.TightenType === undefined) {
          delete baseParams.TightenType;
        }
        if (!baseParams.BeginTime) delete baseParams.BeginTime;
        if (!baseParams.EndTime) delete baseParams.EndTime;
        if (!baseParams.PackCode) delete baseParams.PackCode;

        const first = await api.loadExternalAutoTightenData(baseParams);
        const normalized = this.normalizeListResponse(first);
        let rows = normalized.rows || [];
        const count = normalized.count || rows.length;
        const target = Math.min(count, maxExport);

        while (rows.length < target) {
          const nextPage = Math.floor(rows.length / pageSize) + 1;
          const res = await api.loadExternalAutoTightenData(
            Object.assign({}, baseParams, { Page: nextPage, Limit: pageSize })
          );
          const nextRows = (res && (res.result || res.data || [])) || [];
          if (nextRows.length === 0) break;
          rows = rows.concat(nextRows);
        }

        if (count > maxExport) {
          this.$message({
            message: `数据量过大，仅导出前 ${maxExport} 条`,
            type: "warning",
          });
        }

        return rows.slice(0, target).map((r) =>
          Object.assign({}, r, {
            tightenTypeName: this.getTightenTypeLabel(r.tightenType),
          })
        );
      } finally {
        this.exportLoading = false;
      }
    },
  },
};
</script>

<style scoped>
.operation-column .cell {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 8px;
  white-space: nowrap;
}
</style>
