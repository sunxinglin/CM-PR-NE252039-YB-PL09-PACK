<template>
    <div class="flex-column">

        <div class="filter-container">
            <el-card shadow="never" class="boby-small" style="height: 100%">
                <div slot="header" class="clearfix">
                    <span>用户表</span>
                </div>
                <div>
                    <el-row :gutter="2">

                        <el-col :span="8">

                            <el-input @keyup.enter.native="handleFilter" prefix-icon="el-icon-search" size="small"
                                style="width: 200px; margin-bottom: 0;" class="filter-item" :placeholder="'工号/姓名'"
                                v-model="listQuery.key">
                            </el-input>

                            <el-select class="filter-item" size="small" v-model="listQuery.status"
                                placeholder="请选择用户状态">
                                <el-option v-for="item in logtype" :key="item.key" :label="item.lable"
                                    :value="item.key">
                                </el-option>
                            </el-select>

                        </el-col>

                        <el-col :span="6">

                            <el-button type="infor" size="small" icon="el-icon-search" @click="handleFilter">
                                查询
                            </el-button>

                            <el-button type="primary" icon="el-icon-plus" size="small" @click="handleCreate">
                                添加
                            </el-button>
                            <el-button type="primary" size="small" icon="el-icon-download" @click="importfilebtn">
                                导入用户信息 </el-button>
                            <el-button type="primary" icon="el-icon-upload2" size="small" @click="ModelExpornt">
                                导出用户信息</el-button>
                            <!-- <el-button type="primary"  icon="el-icon-edit" size="small" @click="handleUpdate">编辑</el-button> -->
                            <!-- <el-button type="danger" icon="el-icon-delete" size="small" @click="handleDelete">删除</el-button> -->
                        </el-col>

                    </el-row>
                </div>
            </el-card>
        </div>

        <div class="app-container flex-item">

            <div class="bg-white fh">
                <el-table ref="mainTable" :key='tableKey' :data="list" v-loading="listLoading" border fit stripe
                    highlight-current-row style="width: 100%;" height="calc(100% - 52px)" @row-click="rowClick"
                    @selection-change="handleSelectionChange">
                    <el-table-column align="center" type="selection" width="55">
                    </el-table-column>

                    <el-table-column :label="'Id'" v-if="showDescription" min-width="120px">
                        <template slot-scope="scope">
                            <span>{{ scope.row.id }}</span>
                        </template>
                    </el-table-column>

                    <!--<el-table-column min-width="80px" :label="'账号'">
                        <template slot-scope="scope" >
                            <span  class="link-type" @click="handleUpdate(scope.row)">{{ scope.row.account }}</span>
                        </template>
                    </el-table-column>-->

                    <el-table-column min-width="80px" :label="'姓名'">
                        <template slot-scope="scope">
                            <span class="link-type" @click="handleUpdate(scope.row)">{{ scope.row.name }}</span>
                        </template>
                    </el-table-column>

                    <el-table-column min-width="80px" :label="'工号'">
                        <template slot-scope="scope">
                            <span style='color:red;'>{{ scope.row.workId }}</span>
                        </template>
                    </el-table-column>


                    <el-table-column min-width="80px" v-if='showDescription' :label="'描述'">
                        <template slot-scope="scope">
                            <span style='color:red;'>{{ scope.row.description }}</span>
                        </template>
                    </el-table-column>

                    <el-table-column class-name="status-col" :label="'状态'" min-width="80px">
                        <template slot-scope="scope">
                            <span :class="scope.row.status | statusFilter">
                                {{
                        statusOptions.find(u => u.key ==
                            scope.row.status).display_name
                    }}
                            </span>
                        </template>
                    </el-table-column>

                    <el-table-column align="center" :label="'操作'" min-width="80px"
                        class-name="small-padding fixed-width">
                        <template slot-scope="scope">
                            <el-button type="primary" size="mini" icon="el-icon-edit" @click="handleUpdate(scope.row)">
                                编辑
                            </el-button>
                            <el-button v-if="scope.row.status == 0" size="mini" icon="el-icon-close" type="danger"
                                @click="handleModifyStatus(scope.row, 1)">停用</el-button>
                            <el-button v-else size="mini" icon="el-icon-check" type="success"
                                @click="handleModifyStatus(scope.row, 0)">启用</el-button>
                        </template>
                    </el-table-column>
                </el-table>
                <pagination v-show="total > 0" :total="total" :page.sync="listQuery.page" :limit.sync="listQuery.limit"
                    @pagination="handleCurrentChange" />
            </div>
            <el-dialog class="dialog-mini" width="500px" v-el-drag-dialog :title="textMap[dialogStatus]"
                :visible.sync="dialogFormVisible">
                <el-form :rules="rules" ref="dataForm" :model="temp" label-position="right" label-width="100px">
                    <el-form-item size="small" :label="'Id'" prop="id" v-show="dialogStatus == 'update'">
                        <el-input v-model="temp.id" :disabled="true" size="small" placeholder="系统自动处理"></el-input>
                    </el-form-item>
                    <el-form-item size="small" :label="'账号'" prop="account">
                        <el-input type="password" v-model="temp.account"></el-input>
                    </el-form-item>
                    <el-form-item size="small" :label="'姓名'" prop="name">
                        <el-input v-model="temp.name"></el-input>
                    </el-form-item>
                    <el-form-item size="small" :label="'密码'">
                        <el-input v-model="temp.password"
                            :placeholder="dialogStatus == 'update' ? '如果为空则不修改密码' : '如果为空则默认与账号相同'"
                            show-password></el-input>
                    </el-form-item>
                    <el-form-item size="small" :label="'工号'">
                        <el-input v-model="temp.workId"></el-input>
                    </el-form-item>
                    <el-form-item size="small" :label="'状态'">
                        <el-select class="filter-item" v-model="temp.status" placeholder="Please select">
                            <el-option v-for="item in  statusOptions" :key="item.key" :label="item.display_name"
                                :value="item.key">
                            </el-option>
                        </el-select>
                    </el-form-item>


                </el-form>
                <div slot="footer">
                    <el-button size="mini" @click="dialogFormVisible = false">取消</el-button>
                    <el-button size="mini" v-if="dialogStatus == 'create'" type="primary" @click="createData">
                        确认
                    </el-button>
                    <el-button size="mini" v-else type="primary" @click="updateData">确认</el-button>
                </div>
            </el-dialog>

            <el-dialog width="516px" class="dialog-mini body-small" v-el-drag-dialog :title="'分配角色'"
                :visible.sync="dialogRoleVisible">
                <el-form ref="rolesForm" size="small" v-if="dialogRoleVisible" label-position="left">
                    <el-form-item>
                        <select-roles :roles="selectRoles" :isUnLoadGroupList="true" :userNames="selectRoleNames"
                            v-on:roles-change="rolesChange"></select-roles>
                    </el-form-item>
                </el-form>
                <div slot="footer">
                    <el-button size="mini" @click="dialogRoleVisible = false">取消</el-button>
                    <el-button size="mini" type="primary" @click="acceRole">确认</el-button>
                </div>
            </el-dialog>
            <el-dialog :visible.sync="importfilevisible">
                <el-upload ref="upload" action="string" :auto-upload="false" :on-change="elInFile" :limit="1"
                    :multiple="false" drag accept=".xlsx,.xls">
                    <i class="el-icon-upload"></i>
                    <div class="el-upload__text">将文件拖到此处，或<em>点击选择</em></div>
                </el-upload>
                <el-progress :text-inside="true" :stroke-width="26" :percentage="progressvalue" />
                <div slot="footer">
                    <el-button type="primary" size="small" @click="importfilevisible = false">取消</el-button>

                    <el-button type="primary" size="small" @click="importfile">确认</el-button>
                </div>
            </el-dialog>
        </div>
    </div>

</template>

<script>
import {
    listToTreeSelect
} from '@/utils'
import * as accsssObjs from '@/api/accessObjs'
import * as users from '@/api/users'
import * as apiRoles from '@/api/roles'
import * as login from '@/api/login'
import Treeselect from '@riophae/vue-treeselect'
import '@riophae/vue-treeselect/dist/vue-treeselect.css'
import waves from '@/directive/waves' // 水波纹指令
import Sticky from '@/components/Sticky'
import permissionBtn from '@/components/PermissionBtn'
import SelectRoles from '@/components/SelectRoles'
import Pagination from '@/components/Pagination'
import elDragDialog from '@/directive/el-dragDialog'
import extend from "@/extensions/delRows.js"
import { log } from 'console'

export default {
    name: 'user',
    components: {
        Sticky,
        permissionBtn,
        Treeselect,
        SelectRoles,
        Pagination
    },
    mixins: [extend],
    directives: {
        waves,
        elDragDialog
    },
    data() {
        return {
            defaultProps: { // tree配置项
                children: 'children',
                label: 'label'
            },
            logtype: [
                {
                    key: 0,
                    lable: "正常",
                },
                {
                    key: 1,
                    lable: "停用",
                },
            ],
            multipleSelection: [], // 列表checkbox选中的值
            tableKey: 0,
            list: null,
            total: 0,
            listLoading: true,
            listQuery: { // 查询条件
                page: 1,
                limit: 20,
                key: '',
                status: "",
            },
            apps: [],
            statusOptions: [{
                key: 1,
                display_name: '停用'
            },
            {
                key: 0,
                display_name: '正常'
            }
            ],
            showDescription: false,
            orgs: [], // 用户可访问到的组织列表
            orgsTree: [], // 用户可访问到的所有机构组成的树
            selectRoles: [], // 用户分配的角色
            selectRoleNames: '',
            temp: {
                id: undefined,
                account: '',
                name: '',
                password: '',
                status: 0,
                workId: '',
            },
            dialogFormVisible: false,
            dialogStatus: '',
            textMap: {
                update: '编辑',
                create: '添加'
            },
            dialogRoleVisible: false, // 分配角色对话框
            rules: {
                account: [{
                    required: true,
                    message: '账号不能为空',
                    trigger: 'blur'
                }],
                name: [{
                    required: true,
                    message: '用户名不能为空',
                    trigger: 'blur'
                }]
            },
            downloadLoading: false,
            importfilevisible: false,
            excelfiles: [],
            progressvalue: 0,
        }
    },
    computed: {
        // selectOrgs: {
        //   get: function() {
        //     if (this.dialogStatus === 'update') {
        //       return this.temp.organizationIds && this.temp.organizationIds.split(',')
        //     } else {
        //       return []
        //     }
        //   },
        //   set: function(v) {
        //     var _this = this
        //     _this.temp.organizationIds = v.length > 0 && v.join(',') || ''
        //     var organizations = _this.orgs.filter((val) => {
        //       return _this.temp.organizationIds.indexOf(val.id) >= 0
        //     }).map(u => u.label)
        //     this.temp.organizations = organizations.join(',')
        //   }
        // }
    },
    filters: {
        statusFilter(status) {
            var res = 'color-success'
            switch (status) {
                case 1:
                    res = 'color-danger'
                    break
                default:
                    break
            }
            return res
        }
    },
    created() {
        this.getList()
    },
    mounted() {

    },
    methods: {
        elInFile(f, fs) {
            this.excelfiles = fs;
        },
        rowClick(row) {
            this.$refs.mainTable.clearSelection()
            this.$refs.mainTable.toggleRowSelection(row)
        },
        handleNodeClick(data) {
            this.listQuery.orgId = data.id
            this.getList()
        },
        getAllUsers() {
            this.listQuery.orgId = ''
            this.getList()
        },
        handleSelectionChange(val) {
            this.multipleSelection = val
        },

        getList() {
            this.listLoading = true
            users.getList(this.listQuery).then(response => {
                this.list = response.data
                this.total = response.count
                this.listLoading = false
            })
        },
        handleFilter() {
            this.listQuery.page = 1
            this.getList()
        },
        handleSizeChange(val) {
            this.listQuery.limit = val
            this.getList()
        },
        handleCurrentChange(val) {
            this.listQuery.page = val.page
            this.listQuery.limit = val.limit
            this.getList()
        },
        handleModifyStatus(row, status) { // 模拟修改状态
            this.$message({
                message: '操作成功',
                type: 'success'
            })
            row.status = status
            users.update(row).then(() => { })
        },
        resetTemp() {
            this.temp = {
                id: undefined,
                account: '',
                name: '',
                password: '',
                status: 0
            }
        },
        handleCreate() { // 弹出添加框
            this.resetTemp()
            this.dialogStatus = 'create'
            this.dialogFormVisible = true
            this.$nextTick(() => {
                this.$refs['dataForm'].clearValidate()
            })
        },
        createData() { // 保存提交
            this.$refs['dataForm'].validate((valid) => {
                if (valid) {
                    if (this.temp.password.length == 0) {
                        this.temp.password = this.temp.account
                    }
                    users.add(this.temp).then((response) => {
                        this.getList()
                        this.dialogFormVisible = false
                        this.$notify({
                            title: '成功',
                            message: '创建成功',
                            type: 'success',
                            duration: 2000
                        })
                    })
                }
            })
        },
        handleUpdate(row) { // 弹出编辑框
            this.temp = Object.assign({}, row) // copy obj
            this.temp.password = ""
            this.dialogStatus = 'update'
            this.dialogFormVisible = true
            this.$nextTick(() => {
                this.$refs['dataForm'].clearValidate()
            })
        },
        updateData() { // 更新提交
            this.$refs['dataForm'].validate((valid) => {
                if (valid) {

                    if (this.temp.password.length == 0) {
                        this.temp.password = this.temp.account
                    }
                    const tempData = Object.assign({}, this.temp)
                    users.update(tempData).then(() => {
                        this.getList()
                        this.dialogFormVisible = false
                        this.$notify({
                            title: '成功',
                            message: '更新成功',
                            type: 'success',
                            duration: 2000
                        })
                    })
                }
            })
        },

        handleAccessRole(row) { // 分配角色
            const _this = this
            this.temp = Object.assign({}, row) // copy obj
            apiRoles.loadForUser(this.temp.id).then(response => {
                _this.dialogRoleStatus = 'update'
                _this.dialogRoleVisible = true
                _this.selectRoles = response.result
                _this.getRoleList()
                _this.$nextTick(() => {
                    _this.$refs['rolesForm'].clearValidate()
                })
            })
        },

        // 获取角色
        getRoleList() {
            apiRoles.getList({}).then(response => {
                this.selectRoleNames = [...response.result].filter(x => this.selectRoles.indexOf(x.id) > -1)
                    .map(item => item.name || item.account).join(',')
            })
        },
        rolesChange(type, val) {
            if (type === 'Texts') {
                this.selectRoleNames = val
                return
            }
            this.selectRoles = val
        },
        acceRole() {
            accsssObjs.assign({
                type: 'UserRole',
                firstId: this.temp.id,
                secIds: this.selectRoles
            }).then(() => {
                this.dialogRoleVisible = false
                this.$notify({
                    title: '成功',
                    message: '更新成功',
                    type: 'success',
                    duration: 2000
                })
            })
        },
        importfilebtn() {
            this.importfilevisible = true
            this.progressvalue = 0
        },
        importfile() {
            var form = new FormData();
            if (this.excelfiles.length <= 0) {
                this.$message("请选择文件后重试!");
                return;
            }
            this.$refs.upload.clearFiles();
            form.append("file", this.excelfiles[0].raw);
            this.excelfiles = [];

            this.progressvisible = "visible";
            this.progressvalue = 50
            users
                .InportUser(form)
                .then((reponse) => {
                    this.$notify({
                        title: "成功",
                        message: "导入成功",
                        type: "success",
                        duration: 2000,
                    });
                    this.importfilevisible = false;
                    this.progressvisible = "hidden";
                    this.progressvalue = 0
                    this.stationLoad();
                });
        },
        ModelExpornt() {


            users
                .ExportUser()
                .then((request) => {
                    // this.fullscreenloading = false;
                    this.$notify({
                        title: "提示",
                        message: "数据整理完毕,正在下载",
                        type: "success",
                        duration: 2000,
                    });
                    var blob = new Blob([request], {
                        type: "application/vnd.ms-excel",
                    });
                    var fileName = "用户信息导入导出.xlsx";
                    if (window.navigator.msSaveOrOpenBlob) {
                        navigator.msSaveBlob(blob, fileName);
                    } else {
                        var link = document.createElement("a");

                        link.href = window.URL.createObjectURL(blob);
                        link.download = fileName;
                        link.click();
                        window.URL.revokeObjectURL(link.href);
                    }
                });
        },
    }
}
</script>

<style>
.clearfix:before,
.clearfix:after {
    display: table;
    content: "";
}

.clearfix:after {
    clear: both
}

.el-card__header {
    padding: 12px 20px;
}

.body-small.dialog-mini .el-dialog__body .el-form {
    padding-right: 0px;
    padding-top: 0px;
}
</style>
