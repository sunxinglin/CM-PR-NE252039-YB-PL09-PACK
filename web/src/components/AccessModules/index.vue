<template>

  <div class="compent-dialog-body">
	<div class="p-m">
        <el-form ref="form" label-position="left">
            <el-form-item size="small">
                <div class="block">
					
					
                    <el-tree ref="tree" :data="modulesTree" :check-strictly="true"  show-checkbox node-key="id" default-expand-all :expand-on-click-node="false">
                        <span class="custom-tree-node" slot-scope="{ node }">
                            <span>{{ node.label }}</span>
                        </span>
                    </el-tree>
                </div>
            </el-form-item>
        </el-form>

    </div>
    <div slot="footer" class="el-dialog__footer">
        <el-button size="small" @click="close">取消</el-button>

		 <el-button size="small" type="success" @click="saveAssign">保存</el-button> 
    </div>
</div>

</template>

<script>
    import {
      listToTreeSelect
    } from '@/utils'
    import Treeselect from '@riophae/vue-treeselect'
    import '@riophae/vue-treeselect/dist/vue-treeselect.css'
    import * as login from '@/api/login'
    import * as apiModules from '@/api/modules'
    import * as accessObjs from '@/api/accessObjs'

    export default {
      name: 'access-modules',
      components: {
        Treeselect
      },
      props: ['roleId'],
      data() {
        return {
          currentRoleId: this.roleId,
      modules: [],
      modulesTree: [], // 用户可访问的所有模块组成的树
          menus: [], // 登录用户可以访问到的所有菜单
          roleMenuIds: [], // 角色分配到的菜单,只存ID
          checkModules: [], // 第一步选择的nodes
          noSystemNodes: [],
          step: 1 // 步骤
        }
      },
      watch: {
        roleId(val) { // 因为组件只挂载一次，后期只能通过watch来改变selectusers的值
          this.currentRoleId = val
          this.init()
        },
        step(val) {
          if (val === 1) {
            this.$emit('change-title', '为角色分配【可见模块】')
          } else if (val === 2) {
            this.$emit('change-title', '为角色分配【可见菜单】')
          } else {
            this.$emit('change-title', '为角色分配【可见字段】')
          }
        }
      },
      mounted() {
        var _this = this	
		
        login.getFuncModuleList().then(response => {
          console.log(response)
			_this.modules = response.result.map(function(item) {
				return {
					id: item.id,
					label: item.name,
					parentId: item.parentId
				}
			})
			
			var tmp = JSON.parse(JSON.stringify(_this.modules))
	
			_this.modulesTree = listToTreeSelect(tmp)
this.getRoleModuleIds()
        })
		

      },
      methods: {
      
        filterMenus(moduleId) { // 按模块过滤菜单
          return this.menus.filter(function(menu) {
            return menu.moduleId === moduleId
          })
        },
        getRoleModuleIds() { // 获取角色已分配的模块
          var _this = this
          apiModules.loadForRole(_this.currentRoleId).then(response => {

            _this.$refs.tree.setCheckedKeys(response.result.map(item => item.id))
          })
        },

        onChange(val) {
          console.log(this.roleMenuIds)
        },
        close() {
          this.$emit('close')
        },
        up() {
          this.step = this.step * 1.0 - 1
          return
        },
      
        saveAssign() {
			var checkNodes = this.$refs.tree.getCheckedNodes(true, false)

			this.checkModules = checkNodes
	
			var param={
				roleId:this.currentRoleId,
				selectedModuleId:this.$refs.tree.getCheckedKeys()
			}
			
			
			apiModules.assignRoleFuncModule(param).then(response => {
			    this.$notify({
			      title: '成功',
			      message: '分配成功',
			      type: 'success',
			      duration: 2000
			    })
			  })
          // accessObjs.assign({
          //   type: 'RoleModule',
          //   firstId: this.roleId,
          //   secIds: this.$refs.tree.getCheckedKeys()
          // }).then(resp => {
          //   accessObjs.assign({
          //     type: 'RoleElement',
          //     firstId: this.roleId,
          //     secIds: this.roleMenuIds
          //   }).then(response => {
          //     this.$notify({
          //       title: '成功',
          //       message: '分配成功',
          //       type: 'success',
          //       duration: 2000
          //     })
          //   })
          // })
        }

      }
    }
</script>

<style scoped>
    .custom-tree-node {
        flex: 1;
        display: flex;
        align-items: left;
        justify-content: space-between;
        font-size: 14px;
        padding-right: 8px;
    }

    .p-m {
        padding: 10px;
    }
    .p-l-m{
        padding-left: 10px;
    }

    .hide {
        display: none
    }
</style>