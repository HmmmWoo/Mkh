export default {
  title: '模块中心',
  icon: 'component',
  name: 'admin_module',
  path: '/admin/module',
  permissions: ['admin_Module_Permissions_get'],
  buttons: [],
  component: () => import('./index.vue'),
}
