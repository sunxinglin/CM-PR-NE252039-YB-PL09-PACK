<template>
  <div>
    <div class="app-container">
      <el-row :gutter="4">
        <el-col :span="24">
          <el-card shadow="never" class="boby-small">
            <div slot="header" class="clearfix">
              <span>工艺配方</span>
            </div>
            <div style="margin-bottom: 10px">
              <el-row :gutter="4">
                <el-col :span="22">
                  <el-button
                    type="primary"
                    icon="el-icon-plus"
                    size="mini"
                    @click="handleCreate"
                    >添加</el-button
                  >
                  <el-button
                    type="primary"
                    icon="el-icon-edit"
                    size="mini"
                    @click="handleUpdate"
                    >编辑</el-button
                  >
                  <el-button
                    type="primary"
                    icon="el-icon-delete"
                    size="mini"
                    @click="handleDelete"
                    >删除</el-button
                  >
                </el-col>
                
              </el-row>
            </div>
            <div>
              <el-table
                ref="mainTable"
                :data="datalist"
                v-Lodaing="stepListLodaing"
                row-key="id"
                style="width: 100%"
                :height="tableHeight"
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
                  prop="name"
                  label="名称"
                  min-width="20px"
                  sortable
                  align="center"
                ></el-table-column>
                <!-- <el-table-column prop="name" label="名称" min-width="20px" sortable align="center"></el-table-column>
								<el-table-column prop="description" label="描述" min-width="20px" sortable align="center"></el-table-column> -->
              </el-table>
            </div>
            <div>
              <pagination
                :total="stepTotal"
                v-show="stepTotal > 0"
                :page.sync="stepListQuery.page"
                :limit.sync="stepListQuery.limit"
                @pagination="handleCurrentChange"
              />
            </div>
          </el-card>
        </el-col>
      </el-row>
      <el-dialog
        v-el-drag-dialog
        class="dialog-mini"
        width="500px"
        :title="textMap[dialogStatus]"
        :visible.sync="dialogFormVisible"
      >
        <div>
          <el-form
            :rules="stepRules"
            ref="dataForm"
            :model="data"
            label-position="right"
            label-width="100px"
          >
            
            <el-form-item size="small" :label="'名称'" prop="name">
              <el-input v-model="data.name"></el-input>
            </el-form-item>
            <!-- <el-form-item size="small" :label="'描述'">
              <el-input
                v-model="stepTemp.description"
                :min="1"
                :max="20"
              ></el-input>
            </el-form-item> -->
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
            >确认</el-button
          >
          <el-button size="mini" v-else type="primary" @click="updateData"
            >确认</el-button
          >
        </div>
      </el-dialog>
    </div>
  </div>
</template>

<script>
import * as categoryTypes from "@/api/categoryTypes";

import waves from "@/directive/waves"; // 水波纹指令
import Sticky from "@/components/Sticky";
import permissionBtn from "@/components/PermissionBtn";
import Pagination from "@/components/Pagination";
import elDragDialog from "@/directive/el-dragDialog";
export default {
  name: "categorytype",
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
      tableHeight: 500, // 默认给个高度，后续计算
      stepMultipleSelection: [], //勾选的数据表值
    
      stepTotal: 0, //数据条数
      stepListLodaing: true, //加载特效
      stepListQuery: {
        //查询条件
        page: 1,
        limit: 20,
        key: undefined,
      },
      datalist:[],
      data:{},
      dialogFormVisible: false, //编辑框
      dialogStatus: "", //编辑框功能(添加/编辑)
      textMap: {
        update: "编辑",
        create: "添加",
      },
      stepRules: {
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
      },
    };
  },
  mounted() {
    this.Loda();
    this.$nextTick(() => {
      this.calcTableHeight();
      window.addEventListener('resize', this.calcTableHeight);
    });
  },
  beforeDestroy() {
    window.removeEventListener('resize', this.calcTableHeight);
  },
  methods: {
    calcTableHeight() {
      // 获取浏览器可用高度
      const h = window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight;
      // 获取表格距离顶部的距离
      if (this.$refs.mainTable && this.$refs.mainTable.$el) {
        const topH = this.$refs.mainTable.$el.offsetTop;
        // 减去顶部距离，再减去底部分页等预留空间
        this.tableHeight = h - topH - 100;
      }
    },
    //勾选框
    handleSelectionChange(val) {
      this.stepMultipleSelection = val;
    },
    //关键字搜索
    handleFilter() {
      this.Loda();
    },
    //分页
    handleCurrentChange(val) {
      this.stepListQuery.page = val.page;
      this.stepListQuery.limit = val.limit;
      this.Loda(); //页面加载
    },
    //列表加载
    Loda() {
      this.stepListLodaing = true;
      categoryTypes.loadType().then((response) => {
        console.log(response);
        this.datalist = response.data; //提取数据表
        this.stepTotal = response.count; //提取数据表条数
        this.stepListLodaing = false;
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
    //保存提交
    createData() {
      this.$refs["dataForm"].validate((valid) => {
        if (valid) {
          categoryTypes.addType(this.data).then((response) => {
            this.dialogFormVisible = false; //编辑框关闭
            this.$notify({
              title: "成功",
              message: "创建成功",
              type: "success",
              duration: 2000,
            });
            this.Loda(); //页面加载
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
        this.stepTemp = Object.assign({}, row); //复制选中的数据
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
          categoryTypes.update(this.data).then(() => {
            this.dialogFormVisible = false; //编辑框关闭
            this.$notify({
              title: "成功",
              message: "更新成功",
              type: "success",
              duration: 200,
            });
            this.Loda(); //页面加载
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
          categoryTypes.delType(param).then(() => {
            this.$notify({
              title: "成功",
              message: "删除成功",
              type: "success",
              duration: 2000,
            });
            this.Loda(); //页面加载
          });
        })
        .catch((_) => {});
    },
  },
};
</script>
