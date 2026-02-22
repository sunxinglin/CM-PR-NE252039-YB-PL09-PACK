<template>
  <div class="flex-column">
    <div class="filter-container">
      <el-card shadow="never" class="boby-small" style="height: 100%">
        <div slot="header" class="clearfix">
          <span>生产设备一览表</span>
        </div>
        <div>
          <el-row :gutter="2">
            <el-col :span="21">
              <el-button
                type="primary"
                icon="el-icon-plus"
                size="small"
                @click="handleCreate"
                >添加
              </el-button>
              <el-button
                type="primary"
                icon="el-icon-edit"
                size="small"
                @click="handleUpdate"
                >编辑
              </el-button>
              <el-button
                type="danger"
                icon="el-icon-delete"
                size="small"
                @click="handleDelete"
              >
                删除</el-button
              >
            </el-col>
          </el-row>
        </div>
      </el-card>
    </div>
    <div class="app-container fh">
      <el-table
        ref="mainTable"
        :data="proresourceList"
        v-loading="proresourceListLoading"
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
          align="center"
          width="55"
        ></el-table-column>
        <!-- <el-table-column prop="code" label="编码" min-width="20px" sortable align="center"></el-table-column> -->
        <el-table-column
          prop="stationCode"
          label="工位"
          min-width="20px"
          sortable
          align="center"
        ></el-table-column>
        <el-table-column
          prop="name"
          label="名称"
          min-width="20px"
          sortable
          align="center"
        ></el-table-column>
        <el-table-column
          prop="deviceNo"
          label="设备号"
          min-width="20px"
          sortable
          align="center"
        ></el-table-column>
        <el-table-column
          prop="proResourceType"
          label="设备类型"
          min-width="20px"
          sortable
          align="center"
          :formatter="drivicesetStatus"
       />
         <el-table-column
          prop="deviceBrand"
          label="品牌"
          min-width="20px"
          sortable
          align="center"
          :formatter="drivicesetBrand"
       />
        <el-table-column
          prop="protocolType"
          label="通信协议"
          min-width="20px"
          sortable
          align="center"
          :formatter="setStatus"
        />
       
        <el-table-column
          prop="baud"
          label="波特率"
          min-width="20px"
          sortable
          align="center"
        ></el-table-column>
        <el-table-column
          prop="ipAddress"
          label="IP地址"
          min-width="20px"
          sortable
          align="center"
        ></el-table-column>
        <el-table-column
          prop="port"
          label="端口"
          min-width="20px"
          sortable
          align="center"
        ></el-table-column>
        <el-table-column
          prop="description"
          label="描述"
          min-width="20px"
          sortable
          align="center"
        ></el-table-column>
      </el-table>

      <pagination
        :total="proresourceTotal"
        v-show="proresourceTotal > 0"
        :page.sync="proresourceListQuery.page"
        :limit.sync="proresourceListQuery.limit"
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
          :rules="stepRules"
          ref="dataForm"
          :model="TempData"
          label-position="right"
          label-width="100px"
        >
          <el-form-item size="small" :label="'工位'" prop="stationCode">
            <el-input v-model="TempData.stationCode"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'名称'" prop="name">
            <el-input v-model="TempData.name"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'设备号'" prop="deviceNo">
            <el-input v-model="TempData.deviceNo"></el-input>
          </el-form-item>

          <el-form-item size="small" :label="'设备类型'" prop="proResourceType">
            <el-select size="medium" v-model="TempData.proResourceType" placeholder="请选择">
              <el-option
                v-for="item in driviceoptions"
                :key="item.value"
                :label="item.label"
                :value="item.value"
              >
              </el-option>
            </el-select>
          </el-form-item>
            <el-form-item size="small" :label="'品牌'" >
            <el-select size="medium" v-model="TempData.deviceBrand" placeholder="请选择">
              <el-option
                v-for="item in deviceBrandOptions"
                :key="item.value"
                :label="item.label"
                :value="item.value"
              >
              </el-option>
            </el-select>
          </el-form-item>
          <el-form-item size="small" :label="'通信协议'" prop="protocolType">
            <el-select size="medium" v-model="TempData.protocolType" placeholder="请选择">
              <el-option
                v-for="item in options"
                :key="item.value"
                :label="item.label"
                :value="item.value"
              >
              </el-option>
            </el-select>
          </el-form-item>
          <el-form-item size="small" :label="'波特率'" prop="baud">
            <el-input v-model="TempData.baud"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'IP地址'" prop="baud">
            <el-input v-model="TempData.ipAddress"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'端口'" prop="port">
            <el-input v-model="TempData.port"></el-input>
          </el-form-item>
          <el-form-item size="small" :label="'描述'">
            <el-input
              v-model="TempData.description"
              :min="1"
              :max="20"
            ></el-input>
          </el-form-item>
        </el-form>
      </div>
      <div slot="footer">
        <el-button size="small" @click="dialogFormVisible = false"
          >取消</el-button
        >
        <el-button
          size="small"
          v-if="dialogStatus == 'create'"
          type="primary"
          @click="createData"
          >确认
        </el-button>
        <el-button size="small" v-else type="primary" @click="updateData"
          >确认</el-button
        >
      </div>
    </el-dialog>
  </div>
</template>

<script>
import * as proresources from "@/api/proresource";
import * as axios from "axios";
import waves from "@/directive/waves"; // 水波纹指令
import Sticky from "@/components/Sticky";
import permissionBtn from "@/components/PermissionBtn";
import Pagination from "@/components/Pagination";
import elDragDialog from "@/directive/el-dragDialog";
export default {
  name: "proresource",

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
      stepMultipleSelection: [], //勾选的数据表值
      proresourceList: [], //数据表
      proresourceTotal: 0, //数据条数
      proresourceListLoading: true, //加载特效
      proresourceListQuery: {
        //查询条件
        page: 1,
        limit: 20,
        key: undefined,
      },
      options: [
        {
          value: 1,
          label: "RS232",
        },
        {
          value: 2,
          label: "RS485",
        },
        {
          value: 3,
          label: "ModbusTCP",
        },
        {
          value: 4,
          label: "TCP/IP",
        },
        {
          value: 5,
          label: "Profinet",
        },
      ],
      driviceoptions: [
        {
          value: 1,
          label: "扫码枪",
        },
        {
          value: 2,
          label: "拧紧枪",
        },
        {
          value: 3,
          label: "打印机",
        },
        {
          value: 4,
          label: "电子秤",
        },
        {
          value: 5,
          label: "无线电子秤",
        },
      ],
      deviceBrandOptions:[
              {
          value: 1,
          label: "霍尼韦尔",
        },
        {
          value: 11,
          label: "马头",
        },
        {
          value: 12,
          label: "博世",
        },
        {
          value: 21,
          label: "Anyload",
        },
      ],
      value: 1,
      resourceTypevalue:1,
      TempData: {
        //模块临时值
        id: undefined,
        stationCode: "",
        deviceNo: "",
        name: "",
        baud: 115200,
        protocolType: 1,
        proResourceType:1,
        port: 0,
        ipAddress: "",
        description: "",
        deviceBrand : 1,
      },
      dialogFormVisible: false, //编辑框
      dialogStatus: "", //编辑框功能(添加/编辑)
      textMap: {
        update: "编辑",
        create: "添加",
      },
      stepRules: {
        //编辑框输入限制
        stationCode: [
          {
            required: true,
            message: "工位不能为空",
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
        deviceNo: [
          {
            required: true,
            message: "设备号不能为空",
            trigger: "blur",
          },
        ],
      },
    };
  },
  mounted() {
    this.Load();
  },
  methods: {
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
      this.proresourceListQuery.page = val.page;
      this.proresourceListQuery.limit = val.limit;
      this.Load(); //页面加载
    },

    //列表加载
    Load() {
      this.proresourceListLoading = true;
      proresources.getList(this.proresourceListQuery).then((response) => {
        this.proresourceList = response.data; //提取数据表
        this.proresourceTotal = response.count; //提取数据表条数
        this.proresourceListLoading = false;
      });
    },
    //编辑框数值初始值
    resetTemp() {
      this.TempData = {
        id: undefined,
        stationCode: "",
        deviceNo:"1",
        name: "",
        baud: "115200",
        port: "",
        ipAddress: "192.168.1.30",
        protocolType: 1,
        proResourceType:1,
        description: "",
        deviceBrand : 1,
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
          proresources.add(this.TempData).then((response) => {
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
          proresources.update(this.TempData).then((response) => {
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
          proresources.del(param).then(() => {
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
    setStatus(row) {
      switch (row.protocolType) {
        case 1:
          return "RS232";
        case 2:
          return "RS485";
        case 3:
          return "ModbusTCP";
        case 4:
          return "TCP/IP";
        case 5:
          return "Profinet";
      }
    },
    drivicesetStatus(row) {
      switch (row.proResourceType) {
        case 1:
          return "扫码枪";
        case 2:
          return "拧紧枪";
        case 3:
          return "打印机";
        case 4:
          return "电子秤";
        case 5:
          return "无线电子秤";
      }
    },
    drivicesetBrand(row) {
      switch (row.deviceBrand) {
        case 1:
          return "霍尼韦尔";
        case 11:
          return "马头";
        case 12:
          return "博世";
        case 21:
          return "Anyload";
 
      }
    },
  },
};
</script>
