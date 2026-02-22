<template>
  <div class="flex-column">
    <div class="filter-container">
      <el-card shadow="never" class="boby-small" style="height: 100%">
        <div slot="header" class="clearfix">
          <span>工艺路线配置</span>
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
            <el-col :span="3">
              <el-input @keyup.enter.native="handleFilter" prefix-icon="el-icon-search" size="small"
                style="width: 200px" class="filter-item" :placeholder="'关键字'" v-model="listQuery.key">
              </el-input>
            </el-col>
          </el-row>
        </div>
      </el-card>
    </div>

    <div class="app-container fh">
      <el-table ref="mainTable" :data="list" v-loading="listLoading" row-key="id" border fit stripe
        highlight-current-row style="width: 100%" height="calc(100% - 52px)" @selection-change="handleSelectionChange"
        align="left">
        <el-table-column type="selection" align="center" width="55"></el-table-column>
        <el-table-column min-width="20px" :label="'工艺编码'" prop="code" sortable align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.code }}</span>
          </template>
        </el-table-column>
        <el-table-column min-width="20px" :label="'工艺名称'" prop="name" sortable align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.name }}</span>
          </template>
        </el-table-column>
        <el-table-column min-width="20px" :label="'产出品'" prop="productId" sortable align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.product.name }}</span>
          </template>
        </el-table-column>

        <el-table-column min-width="20px" :label="'描述'" prop="description" sortable align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.description }}</span>
          </template>
        </el-table-column>
      </el-table>

      <pagination v-show="total > 0" :total="total" :page.sync="listQuery.page" :limit.sync="listQuery.limit"
        @pagination="handleCurrentChange" />
    </div>

    <el-dialog v-el-drag-dialog class="dialog-mini" width="1200px" :title="textMap[dialogStatus]"
      :visible.sync="dialogFormVisible">
      <div>
        <el-form :rules="rules" ref="dataForm" :model="temp" label-position="right" label-width="100px" size="small">
          <el-row :gutter="0">
            <el-col :span="6">
              <el-form-item :label="'工艺编码'" prop="code">
                <el-input v-model="temp.code" v-bind:disabled="dialogStatus != 'create'" width="176px">
                </el-input>
              </el-form-item>
            </el-col>
            <el-col :span="6">
              <el-form-item :label="'工艺名称'" prop="name">
                <el-input v-model="temp.name" width="176px"></el-input>
              </el-form-item>
            </el-col>

            <el-col :span="8">
              <el-form-item :label="'产出品'" prop="productId">
                <el-select class="filter-item" style="width: 250px" filterable remote reserve-keyword
                  v-model="temp.productId" placeholder="Please select">
                  <el-option v-for="item in productOptions" :key="item.key" :label="item.display_name"
                    :value="item.key"></el-option>
                </el-select>
              </el-form-item>
            </el-col>
          </el-row>

          <el-row :gutter="0">
            <el-col :span="24">
              <el-form-item :label="'描述'" prop="description">
                <el-input v-model="temp.description"></el-input>
              </el-form-item>
            </el-col>
          </el-row>

          <el-row>
            <label style="font-size: 15px">以下是工艺路线 </label>
          </el-row>

          <el-row>
            <el-col :span="24">
              <div class="super-flow-demo1">
                <div class="node-container">
                  <span class="node-item" v-for="item in nodeItemList" :key="item.value"
                    @mousedown="(evt) => nodeItemMouseDown(evt, item.value)">
                    {{ item.label }}
                  </span>
                </div>
                <div class="flow-container" ref="flowContainer">
                  <super-flow ref="superFlow" :node-list="nodeList" :link-list="linkList" :graph-menu="graphMenu"
                    :node-menu="nodeMenu" :link-menu="linkMenu" :link-base-style="linkBaseStyle" :link-style="linkStyle"
                    :link-desc="linkDesc">
                    <template v-slot:node="{ meta }">
                      <div @mouseup="nodeMouseUp" @click="nodeClick" class="flow-node ellipsis">
                        {{ meta.name }}
                      </div>
                    </template>
                  </super-flow>
                </div>
              </div>
            </el-col>
          </el-row>
        </el-form>
      </div>
      <div slot="footer">
        <el-button size="small" @click="dialogFormVisible = false">取消</el-button>
        <el-button size="small" v-if="dialogStatus == 'create'" type="primary" @click="createData">确认
        </el-button>
        <el-button size="small" v-else type="primary" @click="updateData">确认</el-button>
      </div>
    </el-dialog>
    <el-dialog :title="drawerConf.title" :visible.sync="drawerConf.visible" :close-on-click-modal="false" width="500px">
      <el-form @keyup.native.enter="settingSubmit" @submit.native.prevent v-show="drawerConf.type === drawerType.node"
        ref="nodeSetting" :model="nodeSetting">
        <el-form-item label="节点名称" prop="name">
          <el-input v-model="nodeSetting.name" placeholder="请输入节点名称" maxlength="30">
          </el-input>
        </el-form-item>
        <el-form-item label="节点描述" prop="desc">
          <el-input v-model="nodeSetting.desc" placeholder="请输入节点描述" maxlength="30">
          </el-input>
        </el-form-item>
      </el-form>
      <el-form @keyup.native.enter="settingSubmit" @submit.native.prevent v-show="drawerConf.type === drawerType.link"
        ref="linkSetting" :model="linkSetting">
        <el-form-item label="连线描述" prop="desc">
          <el-input v-model="linkSetting.desc" placeholder="请输入连线描述">
          </el-input>
        </el-form-item>
      </el-form>
      <span slot="footer" class="dialog-footer">
        <el-button @click="drawerConf.cancel"> 取 消 </el-button>
        <el-button type="primary" @click="settingSubmit"> 确 定 </el-button>
      </span>
    </el-dialog>
  </div>
</template>

<script>
import * as flows from "@/api/flow";
import * as steps from "@/api/step";
import * as products from "@/api/product";
import * as axios from "axios";
import waves from "@/directive/waves"; // 水波纹指令
import Sticky from "@/components/Sticky";
import permissionBtn from "@/components/PermissionBtn";
import Pagination from "@/components/Pagination";
import elDragDialog from "@/directive/el-dragDialog";
const drawerType = {
  node: 0,
  link: 1,
};
export default {
  name: "flow",

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
      multipleSelection: [], //勾选的数据表值
      list: [], //数据表
      total: 0, //数据条数
      listLoading: true, //加载特效
      listQuery: {
        //查询条件
        page: 1,
        limit: 20,
        key: undefined,
      },

      stepOptions: [],
      productOptions: [],
      temp: {
        //模块临时值
        id: undefined,
        code: "",
        name: "",
        ProductId: undefined,
        stepList: [],
        description: "",
      },
      dialogChildList: [],
      dialogFormVisible: false, //编辑框
      dialogStatus: "", //编辑框功能(添加/编辑)
      textMap: {
        update: "编辑",
        create: "添加",
      },
      rules: {
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
        productId: [
          {
            required: true,
            message: "请选择产出品",
            trigger: "blur",
          },
        ],
      },
      //路线图开始
      nodeList: [],

      linkList: [],
      drawerType,
      drawerConf: {
        title: "",
        visible: false,
        type: null,
        info: null,
        open: (type, info) => {
          const conf = this.drawerConf;
          conf.visible = true;
          conf.type = type;
          conf.info = info;
          if (conf.type === drawerType.node) {
            conf.title = "节点";
            if (this.$refs.nodeSetting) this.$refs.nodeSetting.resetFields();
            this.$set(this.nodeSetting, "name", info.meta.name);
            this.$set(this.nodeSetting, "desc", info.meta.desc);
          } else {
            conf.title = "连线";
            if (this.$refs.linkSetting) this.$refs.linkSetting.resetFields();
            this.$set(
              this.linkSetting,
              "desc",
              info.meta ? info.meta.desc : ""
            );
          }
        },
        cancel: () => {
          this.drawerConf.visible = false;
          if (this.drawerConf.type === drawerType.node) {
            this.$refs.nodeSetting.clearValidate();
          } else {
            this.$refs.linkSetting.clearValidate();
          }
        },
      },
      linkSetting: {
        desc: "",
      },
      nodeSetting: {
        name: "",
        desc: "",
      },

      dragConf: {
        isDown: false,
        isMove: false,
        offsetTop: 0,
        offsetLeft: 0,
        clientX: 0,
        clientY: 0,
        ele: null,
        info: null,
      },
      nodeItemList: [], //左侧可选择的节点
      //右击菜单
      graphMenu: [
        [
          {
            label: "全选",
            selected: (graph) => {
              graph.selectAll();
            },
          },
        ],
      ],
      //节点右击菜单
      nodeMenu: [
        [
          {
            label: "删除",
            selected: (node) => {
              node.remove();
            },
          },
          {
            label: "编辑",
            selected: (node) => {
              this.drawerConf.open(drawerType.node, node);
            },
          },
        ],
      ],
      //连接线右击菜单
      linkMenu: [
        [
          {
            label: "删除",
            selected: (link) => {
              link.remove();
            },
          },
          {
            label: "编辑",
            selected: (link) => {
              this.drawerConf.open(drawerType.link, link);
            },
          },
        ],
      ],
      //线的样式
      linkBaseStyle: {
        color: "#666666", // line 颜色
        hover: "#FF0000", // line hover 的颜色
        textColor: "#666666", // line 描述文字颜色
        textHover: "#FF0000", // line 描述文字 hover 颜色
        font: "14px Arial", // line 描述文字 字体设置 参考 canvas font
        dotted: false, // 是否是虚线
        lineDash: [4, 4], // 虚线时生效
        background: "rgba(255,255,255,0.6)", // 描述文字背景色
      },
      fontList: ["14px Arial", "italic small-caps bold 12px arial"],
      //路线图结束
    };
  },
  mounted() {
    //路线图开始
    document.addEventListener("mousemove", this.docMousemove);
    document.addEventListener("mouseup", this.docMouseup);
    this.$once("hook:beforeDestroy", () => {
      document.removeEventListener("mousemove", this.docMousemove);
      document.removeEventListener("mouseup", this.docMouseup);
    });
    //路线图结束
    this.getList();
    this.getStepList();
    this.getProductList();
  },
  methods: {
    //勾选框
    handleSelectionChange(val) {
      this.multipleSelection = val;
    },
    //关键字搜索
    handleFilter() {
      this.getList();
    },
    //分页
    handleCurrentChange(val) {
      this.listQuery.page = val.page;
      this.listQuery.limit = val.limit;
      this.getList(); //页面加载
    },
    //列表加载
    getList() {
      this.listLoading = true;
      flows.load(this.listQuery).then((response) => {
        this.list = response.data; //提取数据表
        this.total = response.count; //提取数据表条数
        this.listLoading = false;
      });
    },
    //编辑框数值初始值
    resetTemp() {
      this.temp = {
        id: undefined,
        code: "",
        name: "",
        ProductId: undefined,
        stepList: [],
        description: "",
      };
    },
    //点击添加
    handleCreate() {
      //弹出编辑框
      this.resetTemp(); //数值初始化
      this.dialogChildList = [];
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
          console.log("11", this.$refs.superFlow.toJSON().nodeList);
          console.log("22", this.$refs.superFlow.toJSON().linkList);
          // if (!this.checkupsteplist()) {
          // 	this.$message({
          // 		message: "工序需要填写完成",
          // 		type: "error",
          // 	});
          // 	return;
          // }
          // this.temp.stepList = this.dialogChildList;
          // flows.add(this.temp).then((response) => {
          // 	this.dialogFormVisible = false; //编辑框关闭
          // 	this.$notify({
          // 		title: "成功",
          // 		message: "创建成功",
          // 		type: "success",
          // 		duration: 2000,
          // 	});
          // 	this.getList(); //页面加载
          // });
        }
      });
    },
    //点击编辑
    handleUpdate() {
      if (this.multipleSelection.length !== 1) {
        this.$message({
          message: "只能选中一个进行编辑",
          type: "error",
        });
        return;
      } else {
        var row = this.multipleSelection[0];
        //弹出编辑框
        this.temp = Object.assign({}, row); //复制选中的数据
        this.dialogStatus = "update"; //编辑框功能选择（更新）
        this.dialogFormVisible = true; //编辑框显示
        var param = {
          flowId: row.id,
        };
        flows.getChildList(param).then((response) => {
          this.dialogChildList = response.data;
          this.refreshSelect();
        });
        this.$nextTick(() => {
          this.$refs["dataForm"].clearValidate();
        });
      }
    },
    //更新提交
    updateData() {
      this.$refs["dataForm"].validate((valid) => {
        if (valid) {
          console.log("11", this.$refs.superFlow.toJSON().nodeList);
          console.log("22", this.$refs.superFlow.toJSON().linkList);
          // if (!this.checkupsteplist()) {
          // 	this.$message({
          // 		message: "工序需要填写完成",
          // 		type: "error",
          // 	});
          // 	return;
          // }
          // this.temp.stepList = this.dialogChildList;
          // flows.update(this.temp).then(() => {
          // 	this.dialogFormVisible = false; //编辑框关闭
          // 	this.$notify({
          // 		title: "成功",
          // 		message: "更新成功",
          // 		type: "success",
          // 		duration: 2000,
          // 	});
          // 	this.getList(); //页面加载
          // });
        }
      });
    },
    checkupsteplist() {
      var state = 0;
      this.dialogChildList.forEach((element) => {
        if (element.stepId.length == 0) {
          state++;
        }
      });
      if (state == 0) {
        return true;
      } else {
        return false;
      }
    },
    //点击删除
    handleDelete(row) {
      if (this.multipleSelection.length < 1) {
        this.$message({
          message: "至少删除一个",
          type: "error",
        });
        return;
      }
      this.$confirm("确定要删除吗？")
        .then((_) => {
          var rows = this.multipleSelection;
          var selectids = rows.map((u) => u.id); //提取复选框的数据的Id
          var param = {
            ids: selectids,
          };
          flows.del(param).then(() => {
            this.$notify({
              title: "成功",
              message: "删除成功",
              type: "success",
              duration: 2000,
            });
            this.getList(); //页面加载
          });
        })
        .catch((_) => { });
    },
    getStepList() {
      var _this = this; // 记录vuecomponent
      var param = {};
      steps.getList(param).then((response) => {
        $.each(response.data, function (i, item) {
          _this.nodeItemList.push({
            label: item.code,
            value: () => ({
              width: 120,
              height: 40,
              meta: {
                label: item.code,
                name: item.code,
              },
            }),
          });
        });
      });
    },
    getProductList() {
      var _this = this; // 记录vuecomponent
      var param = {};
      products.getList(param).then((response) => {
        _this.products = response.data.map(function (item, index, input) {
          return {
            key: item.id,
            display_name: item.name,
          };
        });
        _this.productOptions = JSON.parse(JSON.stringify(_this.products));
      });
    },
    handleDetailCreate() {
      // 添加明细

      this.dialogChildList.push({
        id: 0,
        flowId: this.temp.id,
        orderNo: this.dialogChildList.length + 1,
        stepId: "",
      });
      this.refreshSelect();
    },

    handleDetailDelete(id, index, rows) {
      this.$confirm("确定要删除该工序吗？")
        .then((_) => {
          rows.splice(index, 1);
          this.refreshSelect();
        })
        .catch((_) => { });
    },
    handleDetailUpDown(id, index, rows, upDown, event) {
      console.log(event);
      if (event.target.nodeName === "I") {
        event.target.parentNode.blur();
      } else {
        event.target.blur();
      }
      if (
        (index == 0 && upDown == -1) ||
        (index == rows.length - 1 && upDown == 1)
      )
        return;
      var curRow = rows[index];
      var upDownRow = rows[index + upDown];
      var curRowTmpOrderNo = curRow.orderNo;
      curRow.orderNo = upDownRow.orderNo;
      upDownRow.orderNo = curRowTmpOrderNo;
      rows[index] = upDownRow;
      rows[index + upDown] = curRow;
    },
    ChangeSelect(val) {
      this.refreshSelect();
    },
    refreshSelect() {
      const _this = this;
      this.stepOptions.forEach(function (option) {
        option.hasSelect = false;
        _this.dialogChildList.forEach(function (value) {
          if (option.key == value.stepId) option.hasSelect = true;
        });
      });
    },

    //路线图开始
    flowNodeClick(meta) {
      console.log(this.$refs.superFlow.graph);
    },
    linkStyle(link) {
      if (link.meta && link.meta.desc === "1") {
        return {
          color: "red",
          hover: "#FF00FF",
          dotted: true,
        };
      } else {
        return {};
      }
    },
    linkDesc(link) {
      return link.meta ? link.meta.desc : "";
    },
    settingSubmit() {
      const conf = this.drawerConf;
      if (this.drawerConf.type === drawerType.node) {
        if (!conf.info.meta) conf.info.meta = {};
        Object.keys(this.nodeSetting).forEach((key) => {
          this.$set(conf.info.meta, key, this.nodeSetting[key]);
        });
        this.$refs.nodeSetting.resetFields();
      } else {
        if (!conf.info.meta) conf.info.meta = {};
        Object.keys(this.linkSetting).forEach((key) => {
          this.$set(conf.info.meta, key, this.linkSetting[key]);
        });
        this.$refs.linkSetting.resetFields();
      }
      conf.visible = false;
    },
    nodeMouseUp(evt) {
      evt.preventDefault();
    },
    nodeClick() {
      console.log(arguments);
    },
    docMousemove({ clientX, clientY }) {
      const conf = this.dragConf;

      if (conf.isMove) {
        conf.ele.style.top = clientY - conf.offsetTop + "px";
        conf.ele.style.left = clientX - conf.offsetLeft + "px";
      } else if (conf.isDown) {
        // 鼠标移动量大于 5 时 移动状态生效
        conf.isMove =
          Math.abs(clientX - conf.clientX) > 5 ||
          Math.abs(clientY - conf.clientY) > 5;
      }
    },
    docMouseup({ clientX, clientY }) {
      const conf = this.dragConf;
      conf.isDown = false;

      if (conf.isMove) {
        const { top, right, bottom, left } =
          this.$refs.flowContainer.getBoundingClientRect();

        // 判断鼠标是否进入 flow container
        if (
          clientX > left &&
          clientX < right &&
          clientY > top &&
          clientY < bottom
        ) {
          // 获取拖动元素左上角相对 super flow 区域原点坐标
          const coordinate = this.$refs.superFlow.getMouseCoordinate(
            clientX - conf.offsetLeft,
            clientY - conf.offsetTop
          );

          // 添加节点
          this.$refs.superFlow.addNode({
            coordinate,
            ...conf.info,
          });
        }

        conf.isMove = false;
      }

      if (conf.ele) {
        conf.ele.remove();
        conf.ele = null;
      }
    },
    nodeItemMouseDown(evt, infoFun) {
      const { clientX, clientY, currentTarget } = evt;

      const { top, left } = evt.currentTarget.getBoundingClientRect();

      const conf = this.dragConf;
      const ele = currentTarget.cloneNode(true);

      Object.assign(this.dragConf, {
        offsetLeft: clientX - left,
        offsetTop: clientY - top,
        clientX: clientX,
        clientY: clientY,
        info: infoFun(),
        ele,
        isDown: true,
      });

      ele.style.position = "fixed";
      ele.style.margin = "0";
      ele.style.top = clientY - conf.offsetTop + "px";
      ele.style.left = clientX - conf.offsetLeft + "px";

      this.$el.appendChild(this.dragConf.ele);
    },
    //路线图结束
  },
};
</script>

<style lang="less">
.ellipsis {
  white-space: nowrap;
  text-overflow: ellipsis;
  overflow: hidden;
  word-wrap: break-word;
}

.link-base-style-form {
  .el-form-item {
    margin-bottom: 12px;
  }

  padding-bottom: 20px;
  border-bottom: 1px solid #dcdcdc;
}

.super-flow-demo1 {
  margin-top: 20px;
  width: 100%;
  height: 400px;
  background-color: #f5f5f5;
  @list-width: 200px;

  >.node-container {
    width: @list-width;
    float: left;
    height: 100%;
    text-align: center;
    background-color: #ffffff;
  }

  >.flow-container {
    width: calc(100% - @list-width);
    float: left;
    height: 100%;
    overflow: hidden;
  }

  .super-flow__node {
    .flow-node {
      box-sizing: border-box;
      width: 100%;
      height: 100%;
      line-height: 40px;
      padding: 0 6px;
      font-size: 12px;
    }
  }
}

.node-item {
  @node-item-height: 30px;

  font-size: 14px;
  display: inline-block;
  height: @node-item-height;
  width: 120px;
  margin-top: 20px;
  background-color: #ffffff;
  line-height: @node-item-height;
  box-shadow: 1px 1px 4px rgba(0, 0, 0, 0.3);
  cursor: pointer;
  user-select: none;
  text-align: center;
  z-index: 6;

  &:hover {
    box-shadow: 1px 1px 8px rgba(0, 0, 0, 0.4);
  }
}
</style>
