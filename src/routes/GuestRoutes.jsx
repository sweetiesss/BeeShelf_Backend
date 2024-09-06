import { Route, Routes } from "react-router-dom";
import HomePage from "../pages/guest/HomePage";
import { LayoutGuest } from "../component/shared/Layout";

import SignUp from "../component/guest/SignUp";
import SignIn from "../component/guest/SignIn";
import ForgotPassword from "../component/guest/ForgotPassword";

export default function GuestRoutes() {
  return (
    <Routes>
      <Route path="/*" element={<LayoutGuest />}>
        <Route index element={<HomePage />} />
      </Route>
      <Route path="/signin" element={<SignIn />} />
      <Route path="/signup" element={<SignUp />} />
      <Route path="/forgotpassword" element={<ForgotPassword />} />
    </Routes>
  );
}
