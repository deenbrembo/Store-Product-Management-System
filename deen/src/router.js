import { createRouter, createWebHistory } from 'vue-router';
import loginregister from './components/LoginRegister.vue';
import HeadAdminDashboard from './components/HeadAdminDashboard2.vue';
import ManageUser from './components/ManageUser.vue';
import AdminDashboard from './components/AdminDashboard.vue';
import EmployeeDashboard from './components/EmployeeDashboard.vue';
import ManageAdmin from './components/ManageAdmin.vue';
import ManageEmployee from './components/ManageEmployee.vue';
import ManageProfile from './components/ManageProfile.vue';
import ManageProduct from './components/ManageProduct.vue';
import BorrowingPage from './components/EmployeeBorrowingPage.vue';
import ReturnProduct from './components/EmployeeReturnProduct.vue';
import BorrowHistory from './components/EmployeeBorrowHistory.vue';


const routes = [
    { path: '/loginregister', component: loginregister },
    { path: '/headadmin/dashboard', component: HeadAdminDashboard, children : [
        { path: 'manage-user', component: ManageUser },
        { path: 'manage-admin', component: ManageAdmin },
        { path : 'manage-employee', component: ManageEmployee },
        { path : 'manage-profile', component: ManageProfile }
    ] },
    { path: '/admin/dashboard', component: AdminDashboard, children : [
        { path: 'manage-product', component: ManageProduct },
        { path : 'manage-profile', component: ManageProfile }

    ]
    },
    { path: '/employee/dashboard', component: EmployeeDashboard, children : [
        { path : 'manage-profile', component: ManageProfile },
        { path : 'borrowing-item', component: BorrowingPage },
        { path : 'return-item', component: ReturnProduct },
        { path : 'borrow-history', component: BorrowHistory }
    ]
    },
    { path: '/', redirect: '/loginregister' }
];

const router = createRouter({
    history: createWebHistory(),
    routes
});

// Navigation guard to protect routes for authenticated users
router.beforeEach((to, from, next) => {
    const isAuthenticated = localStorage.getItem('authToken');
    if (to.path !== '/loginregister' && !isAuthenticated) {
        next('/loginregister');
    } else {
        next();
    }
});

export default router;

