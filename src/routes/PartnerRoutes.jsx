import { Route, Routes } from "react-router-dom";
import HomePage from "../pages/partner/HomePage";
import { LayoutLogined } from "../component/shared/Layout";
import ProfilePage from "../pages/shared/ProfilePage";
import EditProfilePage from "../pages/shared/EditProfilePage";

export default function PartnerRoutes() {
  return (
    <Routes>
      <Route path="/*" element={<LayoutLogined />}>
        <Route index element={<HomePage />} />
        <Route path="profile" element={<ProfilePage />} />
        <Route path="editProfile" element={<EditProfilePage />} />
      </Route>
    </Routes>
  );
}
