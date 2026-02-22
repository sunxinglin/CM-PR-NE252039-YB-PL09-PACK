<template>
  <div class="flex-column">
    <div class="filter-container">
      <el-card shadow="never" class="boby-small" style="height: 100%">
        <div slot="header" class="clearfix">
          <span>角色表</span>
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
            </el-col>
            <el-col :span="3">
              <el-input
                @keyup.enter.native="handleFilter"
                prefix-icon="el-icon-search"
                size="small"
                style="width: 200px"
                class="filter-item"
                :placeholder="'关键字'"
                v-model="listQuery.key"
              >
              </el-input>
            </el-col>
          </el-row>
        </div>
      </el-card>
    </div>

    <div class="app-container fh">
      <el-table
        ref="mainTable"
        :key="tableKey"
        :data="roleList"
        v-loading="listLoading"
        border
        fit
        stripe
        highlight-current-row
        style="width: 100%"
        height="calc(100% - 1px)"
        @selection-change="handleSelectionChange"
      >
        <el-table-column align="center" type="selection" width="55">
        </el-table-column>

        <el-table-column
          :show-overflow-tooltip="true"
          min-width="200px"
          :label="'角色名称'"
        >
          <template slot-scope="scope">
            <span>{{ scope.row.claimValue }}</span>
          </template>
        </el-table-column>
        <el-table-column
          align="center"
          :label="'操作'"
          :show-overflow-tooltip="true"
          min-width="200px"
          class-name="small-padding fixed-width"
        >
          <template slot-scope="scope">
            <el-button
              type="primary"
              icon="el-icon-user"
              size="small"
              @click="chooseUser(scope.row)"
              style="margin-right: 20px"
              >分配用户</el-button
            >
            <el-button
              type="primary"
              icon="el-icon-menu"
              size="small"
              @click="chooseModule(scope.row)"
              >分配模块</el-button
            >
            <el-button
              type="primary"
              icon="el-icon-edit"
              size="small"
              @click="handleUpdate(scope.row)"
              >编辑
            </el-button>
            <el-button
              type="danger"
              icon="el-icon-delete"
              size="small"
              @click="handleDelete(scope.row)"
              >删除
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </div>

    <el-dialog
      width="500px"
      v-el-drag-dialog
      class="dialog-mini"
      :title="textMap[dialogStatus]"
      :visible.sync="dialogFormVisible"
    >
      <el-form
        :rules="rules"
        ref="dataForm"
        :model="temp"
        label-position="right"
        label-width="100px"
      >
        <el-form-item size="small" :label="'角色名称'" prop="claimValue">
          <el-input v-model="temp.claimValue"></el-input>
        </el-form-item>
      </el-form>
      <div slot="footer">
        <el-button size="small" @click="dialogFormVisible = false"
          >取消</el-button
        >
        <el-button
          size="small"
          v-if="dialogStatus == 'create'"
          type="primary"
          @click="createData"
          >确认</el-button
        >
        <el-button size="small" v-else type="primary" @click="updateData"
          >确认</el-button
        >
      </div>
    </el-dialog>
    <!--只有这么写dialog，才能正常触发ESC关闭-->
    <el-dialog
      class="dialog-mini"
      ref="accessModulesDlg"
      v-el-drag-dialog
      :title="accessTitle"
      :visible.sync="dialogAccessModules"
    >
      <access-modules
        ref="accessModules"
        v-if="dialogAccessModules"
        :role-id="selectRoleId"
        @change-title="changeTitle"
        @close="dialogAccessModules = false"
      >
    </access-modules>
    </el-dialog>
    <!-- 添加角色用户 -->
    <el-dialog
      class="dialog-mini user-dialog"
      v-el-drag-dialog
      :title="'添加角色用户'"
      :visible.sync="roleUsers.dialogUserResource"
    >
      <selectUsersCom
        ref="selectUser"
        v-if="roleUsers.dialogUserResource"
        :hiddenFooter="true"
        :loginKey="'loginUser'"
        :selectUsers.sync="userList"
      >
      </selectUsersCom>
      <div style="text-align: right" slot="footer">
        <el-button
          size="small"
          type="cancel"
          @click="roleUsers.dialogUserResource = false"
          >取消</el-button
        >
        <el-button size="small" type="primary" @click="handleSaveUsers"
          >确定</el-button
        >
      </div>
    </el-dialog>
  </div>
</template>

<script>
import * as users from "@/api/users";
import * as roles from "@/api/roles";
import waves from "@/directive/waves"; // 水波纹指令
import Sticky from "@/components/Sticky";
import RoleUsers from "@/components/RoleUsers";
import permissionBtn from "@/components/PermissionBtn";
import accessModules from "@/components/AccessModules";
import accessResource from "./assignRes";
import Pagination from "@/components/Pagination";
import elDragDialog from "@/directive/el-dragDialog";
import selectUsersCom from "@/components/SelectUsersCom";

export default {
  name: "role",
  components: {
    RoleUsers,
    Sticky,
    permissionBtn,
    accessModules,
    accessResource,
    Pagination,
    selectUsersCom,
  },
  directives: {
    waves,
    elDragDialog,
  },
  data() {
    return {
      defaultProps: {
        // tree配置项
        children: "children",
        label: "label",
      },
      multipleSelection: [], // 列表checkbox选中的值
      tableKey: 0,
      list: null,
      roleList: [],
      userList: null,
      total: 0,
      listLoading: true,
      listQuery: {
        // 查询条件
        page: 1,
        limit: 20,
        key: undefined,
      },
      apps: [],
      statusOptions: [
        {
          key: 1,
          display_name: "停用",
        },
        {
          key: 0,
          display_name: "正常",
        },
      ],
      showDescription: false,
      temp: {
        id: undefined,
        claimValue: "",
      },
      dialogAccessModules: false, // 角色分配模块对话框
      dialogAccessResource: false, // 分配资源对话框
      dialogFormVisible: false,
      dialogStatus: "",
      textMap: {
        update: "编辑",
        create: "添加",
      },
      dialogPvVisible: false,
      pvData: [],
      rules: {
        claimValue: [
          {
            required: true,
            message: "名称不能为空",
            trigger: "blur",
          },
        ],
      },
      downloadLoading: false,
      roleUsers: {
        dialogUserResource: false,
        Texts: "",
        rowIndex: -1,
        selectUsers: [],
        list: [],
      },
      selectRoleId: undefined,
      accessTitle: "为角色分配模块菜单",
    };
  },
  filters: {
    statusFilter(status) {
      var res = "color-success";
      switch (status) {
        case 1:
          res = "color-danger";
          break;
        default:
          break;
      }
      return res;
    },
  },
  created() {
    this.getList();
  },
  methods: {
    changeTitle(val) {
      // 自动调整对话框标题
      this.accessTitle = val;
    },
    rowClick(row) {
      this.$refs.mainTable.clearSelection();
      this.$refs.mainTable.toggleRowSelection(row);
    },
    getAllroles() {
      this.getList();
    },
    handleSelectionChange(val) {
      this.multipleSelection = val;
    },

    getList() {
      this.listLoading = true;
      this.list = [];
      roles.getList(this.listQuery).then((response) => {
        this.roleList = response.result;
        this.listLoading = false;
      });
    },

    handleFilter() {
      this.getList();
    },

    handleModifyStatus(row, status) {
      // 模拟修改状态
      this.$message({
        message: "操作成功",
        type: "success",
      });
      row.status = status;
    },
    resetTemp() {
      this.temp = {
        id: undefined,
        claimValue: "",
      };
    },
    handleCreate() {
      // 弹出添加框
      this.resetTemp();
      this.dialogStatus = "create";
      this.dialogFormVisible = true;
      this.$nextTick(() => {
        this.$refs["dataForm"].clearValidate();
      });
    },
    createData() {
      // 保存提交
      this.$refs["dataForm"].validate((valid) => {
        if (valid) {
          roles.add(this.temp).then((response) => {
            this.getList();
            this.dialogFormVisible = false;
            this.$notify({
              title: "成功",
              message: "创建成功",
              type: "success",
              duration: 2000,
            });
          });
        }
      });
    },
    handleUpdate(row) {
      // 弹出编辑框
     

      this.temp = Object.assign({}, row); // copy obj
      this.dialogStatus = "update";
      this.dialogFormVisible = true;
      this.$nextTick(() => {
        this.$refs["dataForm"].clearValidate();
      });
    },
    updateData() {
      // 更新提交
      this.$refs["dataForm"].validate((valid) => {
        if (valid) {
          const tempData = Object.assign({}, this.temp);
          roles.update(tempData).then(() => {
            this.getList();
            this.dialogFormVisible = false;
            this.$notify({
              title: "成功",
              message: "更新成功",
              type: "success",
              duration: 2000,
            });
          });
        }
      });
    },
    handleDelete(row) {
      // 多行删除
      this.$confirm("确定要删除该用户吗？")
        .then((_) => {
			var delids=[]
			delids.push(row.id)
          var param = {
            ids:delids,
          };
          roles.del(param).then(() => {
            this.$notify({
              title: "成功",
              message: "删除成功",
              type: "success",
              duration: 2000,
            });
            this.getList();
          });
        })
        .catch((_) => {});
    },
    chooseUser(row) {
      this.selectRoleId = row.id;
      var param = {
        roleId: row.id,
      };
      users.getUserList(param).then((response) => {
        this.userList = response.data;
        this.roleUsers.dialogUserResource = true;
      });
    },
    handleSaveUsers() {
      this.$refs.selectUser.handleSaveUsers();
      this.userList.map((u) => u.id);

      const postData = {
        roleId: this.selectRoleId,
        userIds: this.userList.map((u) => u.id),
      };

      roles.AssignRoleUsers(postData).then(() => {
        this.$message.success("添加成功");
        this.roleUsers.dialogUserResource = false;
      });
    },
    chooseModule(row) {
      this.selectRoleId = row.id;
      this.dialogAccessModules = true;
    },
  },
};
</script>

<style lang="scss">
.text {
  font-size: 14px;
}

.item {
  margin-bottom: 18px;
}

.clearfix:before,
.clearfix:after {
  display: table;
  content: "";
}

.clearfix:after {
  clear: both;
}

.el-card__header {
  padding: 12px 20px;
}

.user-dialog {
  .el-dialog {
    height: 70%;

    .el-icon-close {
      padding-top: 10px;
    }

    .el-dialog__body {
      height: calc(100% - 35px - 40px);
    }

    .el-dialog__headerbtn {
      top: 0;
    }
  }
}
</style>
