<template>
  <div class="flex-column">
    <!-- 工位开始 -->
      <div class="filter-container">
        <el-card shadow="never" class="boby-small">
          <div slot="header" class="clearfix">
            <span>工位表</span>
          </div>
          <div>
            <el-row :gutter="4">
              <el-col :span="20">
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
        <el-row :gutter="20">
            <div>
              <el-table ref="stationTable" :data="stationList" :height="tableHeight" v-loading="stationListLoading"
                row-key="id" style="width: 100%" @current-change="handlecurrenttionChange" 
                @selection-change="handleSelectionChange" border fit stripe highlight-current-row align="left">
                <el-table-column type="selection" width="55" align="center">
                </el-table-column>
                <el-table-column prop="code" label="编码" min-width="40px" sortable align="center">
                </el-table-column>
                <el-table-column prop="name" label="名称" min-width="80px" sortable align="center">
                </el-table-column>
                <el-table-column prop="type" label="工位类型" min-width="60px" :formatter="changeStatus" align="center">
                </el-table-column>
                <el-table-column prop="step.name" label="关联工序" min-width="40px" sortable align="center">
                </el-table-column>
                <!-- <el-table-column  prop="description" label="描述" min-width="20px" sortable align="center" ></el-table-column> -->
              </el-table>
            </div>
        </el-row>

      </div>
      <!-- 工位弹框-->
      <el-dialog v-el-drag-dialog class="dialog-mini" width="500px" :title="textMap[dialogStatus]"
        :visible.sync="dialogFormVisible">
        <div>
          <el-form :rules="stationRules" ref="dataForm" :model="stationTemp" label-position="right" label-width="100px">
            <el-form-item size="small" :label="'编码'" prop="code">
							<el-input v-model="stationTemp.code" >
							</el-input>
						</el-form-item>
            <el-form-item size="small" :label="'名称'" prop="name">
              <el-input v-model="stationTemp.name"></el-input>
            </el-form-item>
            <el-form-item size="small" :label="'描述'">
              <el-input v-model="stationTemp.description" :min="1" :max="20"></el-input>
            </el-form-item>
          </el-form>
        </div>
        <div slot="footer">
          <el-button size="mini" @click="dialogFormVisible = false">取消</el-button>
          <el-button size="mini" v-if="dialogStatus == 'create'" type="primary" @click="createData">确认
          </el-button>
          <el-button size="mini" v-else type="primary" @click="updateData">确认</el-button>
        </div>
      </el-dialog>
    </div>
</template>

<script>
import * as stations from "@/api/station";

import * as categoryTypes from "@/api/categoryTypes";
import * as flows from "@/api/flow";
import * as steps from "@/api/step";

import waves from "@/directive/waves"; // 水波纹指令
import Sticky from "@/components/Sticky";
import permissionBtn from "@/components/PermissionBtn";
import Pagination from "@/components/Pagination";
import elDragDialog from "@/directive/el-dragDialog";

export default {
  name: "station",
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
      stationList: [], //数据表
      stationTotal: 0, //数据条数
      flowTotal: 0, //数据条数
      stationListLoading: false, //加载特效
      flowListLoading: false, //加载特效
      tableHeight: null,
      stationListQuery: {
        //查询条件
        flowid: this.stepId,
      },
      flowlistQuery: {
        //查询条件
        page: 1,
        limit: 20,
        key: undefined,
      },
      fullscreenloading: false,
      indexvisible: true,
      importfilevisible: false,
      stationtaskvisible: false,
      progressvalue: 0,
      taskId: 1,

      stationTemp: {
        //模块临时值
        id: undefined,
        code: "",
        name: "",
        stepId: this.stepId,
        description: "",
      },
      stepId: 0,
      dialogFormVisible: false, //编辑框

      dialogStatus: "", //编辑框功能(添加/编辑)
      textMap: {
        update: "编辑",
        create: "添加",
        detail: "任务详情",
      },

      detailData: [],
      flowList: [],
      stationRules: {
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
        type: [
          {
            required: true,
            message: "工位类型不能为空",
            trigger: "blur",
          },
        ],
        stepId: [
          {
            required: true,
            message: "关联工序不能为空",
            trigger: "blur",
          },
        ],
      },

      options: [
        {
          type: 1,
          label: "自动站",
        },
        {
          type: 2,
          label: "线内人工站",
        },
        {
          type: 3,
          label: "线外人工站",
          },
          {
              type: 4,
              label: "可跳过人工站",
          },
      ],
      value: 1,
      sequeceoptions: [
        {
          sequence: 1,
          label: "1",
        },
        {
          sequence: 2,
          label: "2",
        },
        {
          sequence: 3,
          label: "3",
        },
        {
          sequence: 4,
          label: "4",
        },
      ],
      excelfiles: [],
      multipleSelection: [],
      flowtablerowselect: {},
      stationtablerowselect: {}
    };
    
  },
  mounted() {
    
    this.stepId=this.$route.query.stepId;
    this.stationTemp.stepId=this.stepId;
    let h = document.documentElement.clientHeight; // 可见区域高度
    let topH = this.$refs.stationTable.$el.offsetTop; //表格距浏览器顶部距离
    let tableHeight = (h - topH) * 0.74; //表格应该有的高度   乘以多少可自定义
    this.tableHeight = tableHeight;
    this.stationListQuery.stepId=this.stepId;
    this.stationLoad();

    // this.stepsload();
  },
  methods: {
    
    //Bool转换
    formatterBoolean(row, column, cellValue) {
      if (cellValue) {
        return "是";
      } else {
        return "否";
      }
    },

    //关键字搜索
    handleFilter() {
      this.stationLoad();
    },

    //编辑框数值初始值
    resetTemp() {
      this.stationTemp = {
        id: undefined,
        code: "",
        name: "",
        stepId: this.stepId,
        description: "",
      };
     (this.multipleSelection = []);
    },
    handleTaskDetail() {
      if (
        this.stationTemp.id == undefined ||
        this.multipleSelection.length == 0
      ) {
        this.$message({
          message: "请选择需要查看的数据",
          type: "error",
          duration: 5 * 1000,
        });
        return;
      }
      console.log(this.multipleSelection);
      if (this.multipleSelection.length > 1) {
        this.$message({
          message: "请选择单条数据!",
          type: "error",
          duration: 5 * 1000,
        });
        return;
      }
      this.stationtaskvisible = true;
      this.indexvisible = false;
    },

    ///当前的工位的值
    handlecurrenttionChange(val) {
      this.resetTemp();

      this.$refs.stationTable.toggleRowSelection(val);

      console.log(this.multipleSelection);
    },
    handleSelectionChange(val) {
      this.resetTemp();
      this.multipleSelection = val;
     
      if (val.length == 1) {
        this.stationTemp = val[0];
        this.stationTemp.id = val[0].id;
        this.stationTemp.stepId = val[0].stepId;
        this.stationtablerowselect = val[0]
      }
    },
    //列表加载
    stationLoad() {
      this.stationListLoading = true;
      console.log(this.stationListQuery);
      stations.GetStationsByStepId(this.stationListQuery).then((response) => {
        this.stationList = response.data; //提取数据表
        this.stationTotal = response.count; //提取数据表条数
        this.stationListLoading = false;
this.$nextTick(() => {
     this.$refs.stationTable.doLayout();
     // el-table添加ref="tableName"
 });
      });
    },

    //点击添加
    handleCreate() {
      //弹出编辑框
      if (this.stepId==undefined) {
        this.$notify({
          title: "失败",
          message: "请进入工序页面进行操作",
          type: "fail",
          duration: 500,
        });
        return;
      }
      this.resetTemp(); //数值初始化
      this.dialogStatus = "create"; //编辑框功能选择（添加）
      this.dialogFormVisible = true; //编辑框显示

      this.$nextTick(() => {
        this.$refs["dataForm"].clearValidate();
      });
    },

    ///保存提交
    createData() {
      this.$refs["dataForm"].validate((valid) => {
        if (valid) {
          console.log(this.stationTemp);
          stations.add(this.stationTemp).then((response) => {
            this.dialogFormVisible = false; //编辑框关闭
            this.$notify({
              title: "成功",
              message: "创建成功",
              type: "success",
              duration: 2000,
            });
            this.stationLoad(); //页面加载
          });
        }
      });
    },

    //点击编辑
    handleUpdate() {
      if (this.stationTemp.code === "" || this.multipleSelection.length > 1) {
        this.$notify({
          title: "失败",
          message: "请点击一行数据",
          type: "fail",
          duration: 500,
        });
        return;
      }
      this.dialogStatus = "update"; //编辑框功能选择（更新）
      this.value = this.stationTemp.type;
      this.dialogFormVisible = true; //编辑框显示
      this.$nextTick(() => {
        this.$refs["dataForm"].clearValidate();
      });
    },
    //更新提交
    updateData() {
      this.$refs["dataForm"].validate((valid) => {
        if (valid) {
          stations.update(this.stationTemp).then(() => {
            this.dialogFormVisible = false; //编辑框关闭
            this.$notify({
              title: "成功",
              message: "更新成功",
              type: "success",
              duration: 2000,
            });
            this.stationLoad(); //页面加载
          });
        }
      });
    },
    //点击删除
    handleDelete() {
      if (this.stationTemp.id === undefined) {
        this.$message({
          message: "请选择一个想要删除的数据",
          type: "error",
        });
        return;
      }
      this.$confirm("确定要删除吗？")
        .then((_) => {
          var selectids = [];
          selectids.push(this.stationTemp.id); //提取复选框的数据的Id
          var param = {
            ids: selectids,
          };
          stations.del(param).then(() => {
            this.$notify({
              title: "成功",
              message: "删除成功",
              type: "success",
              duration: 2000,
            });
            this.stationLoad(); //页面加载
          });
        })
        .catch((_) => { });
    },

    //#endregion
    changeStatus(row, column, cellValue) {
      switch (cellValue) {
        case 1:
          return "自动站";
        case 2:
          return "线内人工站";
        case 3:
              return "线外人工站";
          case 4:
              return "可跳过人工站";
      }
    },

    LodaType() {
      categoryTypes.loadType().then((response) => {
        this.typeoptions = response.data; //提取数据表
      });
    },
    backflowtableSelect() {
      this.$nextTick(() => {
    
        this.$refs.flowtable.toggleRowSelection(this.flowtablerowselect);
        this.$refs.stationTable.toggleRowSelection(this.stationtablerowselect);

      })

    },
  },
};
</script>
