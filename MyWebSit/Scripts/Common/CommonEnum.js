/* 管理平台模块框架页面脚本常量 */
var ManagementFrameIndex = {
    /* 添加选项卡页面主窗口函数常量 */
    AddMainWindow: {
        /* 选项卡激活形式 */
        ActiveStyle: {
            NOT_ACTIVE: "0", /* 不激活新窗口 */
            ACTIVE: "1" /* 激活新窗口 */
        },
        /* 选项卡顺序形式 */
        OrderStyle: {
            LAST: "0", /* 添加至最后的选项卡标签之后 */
            NEXT: "1" /* 添加至当前激活的选项卡之后 */
        }
    },
    /* 选项卡右键菜单项点击事件函数常量 */
    TabContextMenuItemClick: {
        /* 菜单项操作类型 */
        OperationType: {
            REFRESH_CURRENT_TAB: "0", /* 刷新标签页 */
            CLOSE_CURRENT_TAB: "1", /* 关闭标签页 */
            CLOSE_OTHER_TAB: "2", /* 关闭其他标签页 */
            CLOSE_ALL_TAB: "3" /* 关闭全部标签页【首页除外】 */
        }
    }
};
