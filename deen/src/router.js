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
import EmpBorrowingPage from './components/EmployeeBorrowingPage.vue';
import EmpReturnProduct from './components/EmployeeReturnProduct.vue';
import AdmBorrowingPage from './components/AdminBorrowingPage.vue';
import AdmReturnProduct from './components/AdminReturnProduct.vue';
import BorrowHistory from './components/BorrowHistory.vue';
import BorrowedItem from './components/BorrowedItem.vue';


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
        { path : 'manage-profile', component: ManageProfile },
        { path : 'borrowing-item', component: AdmBorrowingPage },
        { path : 'return-item', component: AdmReturnProduct },
        { path : 'borrow-history', component: BorrowHistory },
        { path : 'borrowed-item', component: BorrowedItem }

    ]
    },
    { path: '/employee/dashboard', component: EmployeeDashboard, children : [
        { path : 'manage-profile', component: ManageProfile },
        { path : 'borrowing-item', component: EmpBorrowingPage },
        { path : 'return-item', component: EmpReturnProduct },
        { path : 'borrow-history', component: BorrowHistory },
        { path : 'borrowed-item', component: BorrowedItem }
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

