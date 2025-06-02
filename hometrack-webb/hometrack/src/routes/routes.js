import config from "../config/index";

// Layouts
import HomeLayout from "../layouts/homeLayout/homeLayout";

// Pages

import Login from "../pages/login/index";
import Home from "../pages/home/index";
import System from "../pages/system";
import User from "../pages/user/index";
import Monitoring from "../pages/monitoring/index";
import Dashboard from "../pages/dashboard/index";
import Content from "../pages/content/index";

const publicRoutes = [
  { path: config.routes.home, component: Home, layout: HomeLayout },
  { path: config.routes.login, component: Login , layout: null },
  { path: config.routes.system, component: System },
  { path: config.routes.user, component: User },
  { path: config.routes.monitoring, component: Monitoring },
  { path: config.routes.dashboard, component: Dashboard },
  { path: config.routes.content, component: Content },
];

const privateRoutes = [];

export { publicRoutes, privateRoutes };
