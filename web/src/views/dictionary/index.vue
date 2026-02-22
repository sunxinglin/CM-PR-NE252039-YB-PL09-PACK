<template>
	<div class="flex-column">
	<el-row :gutter="4" style="height: 100%">
			<el-col :span="12" style="height: 100%">
				<div class="filter-container">
					<el-card shadow="never" class="boby-small" style="height: 100%">
						<div slot="header" class="clearfix">
							<span>数字字典</span>
						</div>
						<div>
							<el-row :gutter="2">
								<el-col :span="18">
									<el-button type="primary" icon="el-icon-plus" size="small" @click="handleDictionaryCreate">添加</el-button>
									<el-button type="primary" icon="el-icon-edit" size="small" @click="handleDictionaryUpdate">编辑</el-button>
									<el-button type="danger" icon="el-icon-delete" size="small" @click="handleDictionaryDelete">删除</el-button>
								</el-col>
								<!-- <el-col :span="6">
									<el-input @keyup.enter.native="handleDictionaryFilter" prefix-icon="el-icon-search" size="small" style="width:180px"
									 class="filter-item" :placeholder="'关键字'" v-model="dictionaryListQuery.key"></el-input>
								</el-col> -->
							</el-row>
						</div>
							</el-card>
						</div>
						<div class="app-container fh" style=" padding: 0px;">
							<el-table ref="mainTable" :data="dictionaryList" v-loading="dictionaryListLoading" row-key="id" border fit
							 stripe highlight-current-row style="width: 100%;" height="calc(100% - 150px)" 
							 @current-change="handleDictionarycurrentChange" @selection-change="handleDictionarySelectionChange" align="left">
								<el-table-column type="selection" mini-width="100px" align="center"></el-table-column>
								<el-table-column prop="code" label="编号" mini-width="100px" sortable align="center"></el-table-column>
								<el-table-column prop="name" label="名称" mini-width="100px" sortable align="center"></el-table-column>
								<el-table-column prop="description" label="描述" mini-width="100px" sortable align="center"></el-table-column>
							</el-table>
						
							<pagination :total="dictionaryTotal" v-show="dictionaryTotal>0" :page.sync="dictionaryListQuery.page"
							 :limit.sync="dictionaryListQuery.limit" @pagination="handleDictionaryCurrentChange" />
						</div>
						</el-col>
				<el-col :span="12" style="height: 100%">
				<div class="filter-container">
					<el-card shadow="never" class="boby-small" style="height: 100%">
						<div slot="header" class="clearfix">
							<span>数字字典详情表</span>
					</div>
					<div>
						<el-row :gutter="2">
								<el-col :span="18">
									<el-button type="primary" icon="el-icon-plus" size="small" @click="handleDictionaryDetailCreate">添加</el-button>
									<el-button type="primary" icon="el-icon-edit" size="small" @click="handleDictionaryDetailUpdate">编辑</el-button>
									<el-button type="danger" icon="el-icon-delete" size="small" @click="handleDictionaryDetailDelete">删除</el-button>
								</el-col>
								<!-- <el-col :span="6">
									<el-input @keyup.enter.native="handleDictionaryDetailFilter" prefix-icon="el-icon-search" size="small" style="width:180px"
									 class="filter-item" :placeholder="'关键字'" v-model="dictionaryDetailListQuery.key"></el-input>
								</el-col> -->
							</el-row>
						</div>
							</el-card>
						</div>
						<div class="app-container fh" style=" padding: 0px;">
							<el-table ref="subTables" :data="dictionaryDetailList" v-loading="dictionaryDetailListLoading" row-key="id"
							 border fit stripe highlight-current-row style="width: 100%;" height="calc(100% - 150px)" @selection-change="handleDictionaryDetailSelectionChange"
							 align="left">
								<el-table-column type="selection" mini-width="100px" align="center"></el-table-column>
								<el-table-column prop="code" label="编号" mini-width="100px" sortable align="center"></el-table-column>
								<el-table-column prop="name" label="名称" mini-width="100px" sortable align="center"></el-table-column>
								<el-table-column prop="description" label="描述" mini-width="100px" sortable align="center"></el-table-column>
								<el-table-column prop="dictionaryId" label="数字字典表" mini-width="100px" sortable align="center"></el-table-column>
								<el-table-column prop="value" label="值" mini-width="100px" sortable align="center"></el-table-column>
							</el-table>
						
							<pagination :total="dictionaryDetailTotal" v-show="dictionaryDetailTotal>0" :page.sync="dictionaryDetailListQuery.page"
							 :limit.sync="dictionaryDetailListQuery.limit" @pagination="handleDictionaryDetailCurrentChange" />
						</div>
					
				</el-col>
			</el-row>
			<el-dialog v-el-drag-dialog class="dialog-mini" width="500px" :title="textMap[dialogStatus]" :visible.sync="dialogFormVisible">
				<div>
					<el-form v-show="dictionaryOrDictionaryDetail==false" :rules="dictionaryRules" ref="dataForm1" :model="temp"
					 label-position="right" label-width="100px">
						<el-form-item size="small" :label="'编号'" prop="code">
							<el-input v-model="temp.code"></el-input>
						</el-form-item>
						<el-form-item size="small" :label="'名称'" prop="name">
							<el-input v-model="temp.name"></el-input>
						</el-form-item>
						<el-form-item size="small" :label="'描述'">
							<el-input v-model="temp.description" :min="1" :max="20"></el-input>
						</el-form-item>
					</el-form>
					<el-form v-show="dictionaryOrDictionaryDetail==true" :rules="dictionaryDetailRules" ref="dataForm2" :model="temp"
					 label-position="right" label-width="100px">
						<el-form-item size="small" :label="'编号'" prop="code">
							<el-input v-model="temp.code"></el-input>
						</el-form-item>
						<el-form-item size="small" :label="'名称'" prop="name">
							<el-input v-model="temp.name"></el-input>
						</el-form-item>
						<el-form-item size="small" :label="'描述'">
							<el-input v-model="temp.description" :min="1" :max="20"></el-input>
						</el-form-item>
						<el-form-item size="small" :label="'所属数字字典'">
							<el-select v-model="temp.dictionaryId" placeholder="请选择" filterable>
								<el-option v-for="item in dictionaryData" filterable allow-create :key="item.id" :label="item.code+'-'+item.name"
								 :value="item.id">
								</el-option>
							</el-select>
						</el-form-item>
						<el-form-item size="small" :label="'值'" prop="value">
							<el-input v-model="temp.value" :min="1" :max="20"></el-input>
						</el-form-item>
					</el-form>
				</div>
				<div slot="footer">
					<el-button size="mini" @click="dialogFormVisible = false">取消</el-button>
					<el-button v-show="dictionaryOrDictionaryDetail==false" size="mini" v-if="dialogStatus=='create'" type="primary"
					 @click="createDictionaryData">确认</el-button>
					<el-button v-show="dictionaryOrDictionaryDetail==false" size="mini" v-else type="primary" @click="updateDictionaryData">确认</el-button>
					<el-button v-show="dictionaryOrDictionaryDetail==true" size="mini" v-if="dialogStatus=='create'" type="primary"
					 @click="createDictionaryDetailData">确认</el-button>
					<el-button v-show="dictionaryOrDictionaryDetail==true" size="mini" v-else type="primary" @click="updateDictionaryDetailData">确认</el-button>
				</div>
			</el-dialog>
		</div>
	
</template>

<script>
import * as dictionarys from "@/api/dictionary";
import * as dictionaryDetails from "@/api/dictionaryDetail";
import * as axios from "axios";
import waves from "@/directive/waves"; // 水波纹指令
import Sticky from "@/components/Sticky";
import permissionBtn from "@/components/PermissionBtn";
import Pagination from "@/components/Pagination";
import elDragDialog from "@/directive/el-dragDialog";
export default {
  name: "dictionary",

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
      dictionaryMultipleSelection: [], //主表复选框
      dictionaryList: [], // 列表
      dictionaryTotal: 0, //列表数据数目
      dictionaryListLoading: true, //主表加载特效
dictionarytemp:{},
      dictionaryDetailMultipleSelection: [], //子表复选框
      dictionaryDetailList: [], // 列表
      dictionaryDetailTotal: 0, //列表数据数目
      dictionaryDetailListLoading: false, //子表加载特效

      dictionaryData: [], //主表的所有数据存放
      dictionaryListQuery: {
        // 主表查询条件
        page: 1,
        limit: 20,
        key: undefined,
      },
      dictionaryDetailListQuery: {
        // 子表查询条件
        page: 1,
        limit: 20,
        key: undefined,
      },
      temp: {
        // 模块临时值
        id: undefined,
        code: "",
        name: "",
        description: "",
        dictionary: [],
        dictionaryId: "",
        value: "",
      },
      dialogFormVisible: false, // 编辑框
      dialogStatus: "", //编辑框功能选择
      dictionaryOrDictionaryDetail: false, //主表与子表选择
      textMap: {
        update: "编辑",
        create: "添加",
      },
      //主表信息输入限制
      dictionaryRules: {
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
      //子表信息输入限制
      dictionaryDetailRules: {
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
        dictionaryId: [
          {
            required: true,
            message: "数字字典表不能为空",
            trigger: "blur",
          },
        ],
        value: [
          {
            required: true,
            message: "值不能为空",
            trigger: "blur",
          },
        ],
      },
    };
  },
  mounted() {
    this.loadDictionary(); //主表加载
    // this.loadDictionaryDetail(); //子表加载
  },
  methods: {
    handleDictionarySelectionChange(val){
      this.dictionaryMultipleSelection=val
    },
    //主表点击
    handleDictionarycurrentChange(val) {
		console.log(1111);
		this.dictionarytemp=val
		this.loadDictionaryDetail()
	},
    //主表关键字搜索
    handleDictionaryFilter() {
      this.loadDictionary(); //主表加载
    },
    //主表分页
    handleDictionaryCurrentChange(val) {
      this.dictionaryListQuery.page = val.page;
      this.dictionaryListQuery.limit = val.limit;
      this.loadDictionary(); //主表加载
    },
    //主表加载
    loadDictionary() {
      this.dictionaryListLoading = true;
      dictionarys.load(this.dictionaryListQuery).then((response) => {
        this.dictionaryList = response.data; //提取数据
        this.dictionaryTotal = response.count; //提取数据的条数
        this.dictionaryListLoading = false;
      });
    },
    //子表复选框
    handleDictionaryDetailSelectionChange(val) {
      this.dictionaryDetailMultipleSelection = val;
    },
    //子表关键字搜索
    handleDictionaryDetailFilter() {
      this.loadDictionaryDetail();
    },
    //子表分页
    handleDictionaryDetailCurrentChange(val) {
      this.dictionaryDetailListQuery.page = val.page;
      this.dictionaryDetailListQuery.limit = val.limit;
      this.loadDictionaryDetail();
    },
    //子表加载
    loadDictionaryDetail() {
      this.dictionaryDetailListLoading = true;
	  this.dictionaryDetailListQuery.key=this.dictionarytemp.id
      dictionaryDetails
        .LoadDetail(this.dictionaryDetailListQuery)
        .then((response) => {
          this.dictionaryDetailList = response.data; //提取数据
          this.dictionaryDetailTotal = response.count; //提取数据条数
          this.dictionaryDetailListLoading = false;
        });
    },
    //查询主表所有数据
    getDictionaryList() {
      dictionarys.getList().then((response) => {
        this.dictionaryData = response.data; //提取主表的数据
      });
    },
    //弹窗数值初始化
    resetTemp() {
      this.temp = {
        id: undefined,
        code: "",
        name: "",
        description: "",
        dictionaryId: "",
        value: "",
      };
    },
    //主表点击添加
    handleDictionaryCreate() {
      // 弹出添加框
      this.resetTemp();
      this.dialogStatus = "create";
      this.dialogFormVisible = true;
      this.dictionaryOrDictionaryDetail = false;
      this.$nextTick(() => {
        this.$refs["dataForm1"].clearValidate();
      });
    },
    //子表点击添加
    handleDictionaryDetailCreate() {
      // 弹出添加框
      this.resetTemp(),
        this.getDictionaryList(),
        (this.dialogStatus = "create");
      this.dialogFormVisible = true;
      (this.dictionaryOrDictionaryDetail = true),
        this.$nextTick(() => {
          this.$refs["dataForm2"].clearValidate();
        });
    },
    // 主表保存提交
    createDictionaryData() {
      this.$refs["dataForm1"].validate((valid) => {
        if (valid) {
          dictionarys.add(this.temp).then((response) => {
            this.dialogFormVisible = false; //编辑框关闭
            this.$notify({
              title: "成功",
              message: "创建成功",
              type: "success",
              duration: 2000,
            });
            this.loadDictionary(); //主表加载
          });
        }
      });
    },
    // 子表保存提交
    createDictionaryDetailData() {
      this.$refs["dataForm2"].validate((valid) => {
        if (valid) {
          //查询主表的数据
          dictionarys.get(this.temp.dictionaryId).then((response) => {
            this.temp.dictionary = response.data;
          }),
            //根据主表的的数据来添加子表
            dictionaryDetails.add(this.temp).then((response) => {
              this.dialogFormVisible = false; //编辑框关闭
              this.$notify({
                title: "成功",
                message: "创建成功",
                type: "success",
                duration: 2000,
              });
              this.loadDictionaryDetail(); //子表加载
            });
        }
      });
    },
    //主表点击编辑
    handleDictionaryUpdate() {
      if (this.dictionaryMultipleSelection.length !== 1) {
        this.$message({
          message: "只能选中一个进行编辑",
          type: "error",
        });
        return;
      } else {
        var row = this.dictionaryMultipleSelection[0];
        // 弹出编辑框
        this.temp = Object.assign({}, row); //复制选中的表数据
        this.dialogStatus = "update"; //选择功能为更新
        this.dialogFormVisible = true; //编辑框显示
        this.dictionaryOrDictionaryDetail = false; //选择编辑框为主表编辑框
        this.$nextTick(() => {
          this.$refs["dataForm1"].clearValidate();
        });
      }
    },
    //子表点击编辑
    handleDictionaryDetailUpdate() {
      if (this.dictionaryDetailMultipleSelection.length !== 1) {
        this.$message({
          message: "只能选中一个进行编辑",
          type: "error",
        });
        return;
      } else {
        var row = this.dictionaryDetailMultipleSelection[0];
        // 弹出编辑框
        this.temp = Object.assign({}, row); // 复制选中表数据
        this.dialogStatus = "update"; //选择功能为更新
        this.dialogFormVisible = true; //编辑框显示
        this.dictionaryOrDictionaryDetail = true; //选择编辑框为子表
        this.$nextTick(() => {
          this.$refs["dataForm2"].clearValidate();
        });
      }
    },
    // 主表更新提交
    updateDictionaryData() {
      this.$refs["dataForm1"].validate((valid) => {
        if (valid) {
          dictionarys.update(this.temp).then(() => {
            this.dialogFormVisible = false; //编辑框关闭
            this.$notify({
              title: "成功",
              message: "更新成功",
              type: "success",
              duration: 2000,
            });
            this.loadDictionary(); //主表加载
          });
        }
      });
    },
    // 子表更新提交
    updateDictionaryDetailData() {
      this.$refs["dataForm2"].validate((valid) => {
        if (valid) {
          dictionaryDetails.update(this.temp).then(() => {
            this.dialogFormVisible = false; //编辑框关闭
            this.$notify({
              title: "成功",
              message: "更新成功",
              type: "success",
              duration: 2000,
            });
            this.loadDictionaryDetail(); //子表加载
          });
        }
      });
    },
    //主表删除
    handleDictionaryDelete(row) {
      if (this.dictionaryMultipleSelection.length < 1) {
        this.$message({
          message: "至少删除一个",
          type: "error",
        });
        return;
      }
      this.$confirm("确定要删除吗？")
        .then((_) => {
          var rows = this.dictionaryMultipleSelection;
          var selectids = rows.map((u) => u.id);
          var param = {
            ids: selectids,
          };
          dictionarys.del(param).then(() => {
            this.$notify({
              title: "成功",
              message: "删除成功",
              type: "success",
              duration: 2000,
            });
            this.loadDictionary(); //主表加载
          });
        })
        .catch((_) => {});
    },
    //子表删除
    handleDictionaryDetailDelete(row) {
      if (this.dictionaryDetailMultipleSelection.length < 1) {
        this.$message({
          message: "至少删除一个",
          type: "error",
        });
        return;
      }
      this.$confirm("确定要删除吗？")
        .then((_) => {
          var rows = this.dictionaryDetailMultipleSelection;
          var selectids = rows.map((u) => u.id);
          var param = {
            ids: selectids,
          };
          dictionaryDetails.del(param).then(() => {
            this.$notify({
              title: "成功",
              message: "删除成功",
              type: "success",
              duration: 2000,
            });
            this.loadDictionaryDetail(); //加载子表
          });
        })
        .catch((_) => {});
    },
  },
};
</script>
