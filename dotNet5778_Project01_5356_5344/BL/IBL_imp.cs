﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using BE;
using DAL;
using GoogleMapsApi;
using GoogleMapsApi.Entities.Directions.Request;
using GoogleMapsApi.Entities.Directions.Response;
using GoogleMapsApi.Entities.Common;
using GoogleMapsApi.Entities.Elevation.Request;
using GoogleMapsApi.Entities.Geocoding.Request;
using GoogleMapsApi.Entities.Geocoding.Response;
using GoogleMapsApi.StaticMaps;
using GoogleMapsApi.StaticMaps.Entities;
using System.Runtime;
using System.Runtime.InteropServices;
using GoogleMapsApi.Entities.PlaceAutocomplete.Request;
//using System.Runtime.InteropServices;
namespace BL
{
    /// <summary>
    /// implantation for all BL methods
    /// this layer is in charge of taking care of all the logical process in this program
    /// each function makes sure all is equit and then sends to bottom level - DAL
    /// </summary>
    public class IBL_imp : IBL
    {
        /// <summary>
        /// the DAL from factory
        /// </summary>
        public Idal myDal = DalFactory.Get_DAL;

        /// <summary>
        /// adds a nanny to archive of nanny's
        /// </summary>
        /// <param name="nanny"></param>
        public void addNanny(Nanny nanny)
        {
            // if this id number is already used
            if (IdExist(nanny.id))
                throw new Exception("Sorry, but this ID number is already in the system.");


            // if nanny is not over the age of 18 she cannot work
            if (DateTime.Now.CompareTo(nanny.birthday.AddYears(18)) >= 0)
                myDal.addNanny(nanny);
            else
                throw new Exception("Joining the system is prohibited under the age of 18");

        }

        /// <summary>
        /// removes a nanny from the list, and all contracts associated with nanny.
        /// using DAL method
        /// </summary>
        /// <param name="nanny"></param>
        public void deleteNanny(Nanny nanny)
        {
            IEnumerable<Contract> nannyContracts = ListOfContractsById(nanny.id);
            if (nannyContracts != null)
            {
                foreach (Contract c in nannyContracts.ToList())
                    deleteContract(c);
            }

            myDal.deleteNanny(nanny);
        }

        /// <summary>
        /// gets a nanny and swaps her info with nanny matching her id.
        /// using DAL method
        /// </summary>
        /// <param name="nanny"></param>
        public void updateNanny(Nanny nanny)
        {
            myDal.updateNanny(nanny);
        }

        /// <summary>
        /// adds a mother to the archive
        /// using DAL method
        /// </summary>
        /// <param name="mother"></param>
        public void addMother(Mother mother)
        {
            // if this id number is already used
            if (IdExist(mother.id))
                throw new Exception("Sorry, but this ID number is already in the system.");

            myDal.addMother(mother);
        }

        /// <summary>
        /// removes a mother from archive
        /// using DAL method
        /// </summary>
        /// <param name="mother"></param>
        public void deleteMother(Mother mother)
        {
            //deletes all children => all contracts => decreasing Nanny number of signed contracts
            IEnumerable<Child> childrenOfMother = getListOfChildByMother(mother);
            if (childrenOfMother != null)
            {
                foreach (Child c in childrenOfMother)
                    deleteChild(c);
            }

            myDal.deleteMother(mother);
        }

        /// <summary>
        /// updates mothers info by 
        /// using DAL method
        /// </summary>
        /// <param name="mother"></param>
        public void updateMother(Mother mother)
        {
            myDal.updateMother(mother);
        }

        /// <summary>
        /// adds a new child to archive
        /// using DAL method
        /// </summary>
        /// <param name="child"></param>
        public void addChild(Child child)
        {
            // if this id number is already used
            if (IdExist(child.id))
                throw new Exception("Sorry, but this ID number is already in the system.");

            if (DateTime.Now.CompareTo(child.birthday.AddMonths(3)) >= 0)
                myDal.addChild(child);
            else
                throw new Exception("Your child is to young to be send to a nanny.\n");
        }

        /// <summary>
        /// removes a child from archive
        /// using DAL method
        /// </summary>
        /// <param name="child"></param>
        public void deleteChild(Child child)
        {
            if (ListOfContractsById(child.id).Any())
            {
                Contract child_contract = ListOfContractsById(child.id).ElementAt(0);
                deleteContract(child_contract);
            }

            myDal.deleteChild(child);
        }

        /// <summary>
        /// updates child information
        /// using DAL method
        /// </summary>
        /// <param name="child"></param>
        public void updateChild(Child child)
        {

            myDal.updateChild(child);
        }

        /// <summary>
        /// adds a contract by checking if all info is good
        /// </summary>
        /// <param name="contract"></param>
        public void addContract(Contract contract)
        {
            Child child = getChildByID(contract.childId);

            if (!myDal.isChildInList(child.id))
            {
                throw new Exception("cannot sign contract:\n child is not exist in DS!.\nthose are the contract detail:\n" +
      "***********************************************\n" + contract.ToString() + "***********************************************\n");
            }

            // checks that age of child is over 3 months
            if (DateTime.Now.CompareTo(child.birthday.AddMonths(3)) <= 0)
            {
                throw new Exception("cannot sign contract:\nage of child is less then three months.\nthose are the contract detail:\n" +
                    "***********************************************\n" + contract.ToString() + "***********************************************\n");
            }

            Nanny nanny = GetNannyByID(contract.NannysId);

            if (nanny == null)
            {
                throw new Exception("cannot sign contract:\n nanny is not exist in DS!.\nthose are the contract detail:\n" +
      "***********************************************\n" + contract.ToString() + "***********************************************\n");
            }

            // checks if nanny is not over her max number of kids
            if (nanny.numberOfSignedContracts == nanny.maxOfKids)
            {
                throw new Exception("cannot sign contract:\nnanny is full.\nthose are the contract detail:\n" +
                    "***********************************************\n" + contract.ToString() + "***********************************************\n");
            }

            // checks that the child is in a age that nanny can take
            if (DateTime.Now.CompareTo(child.birthday.AddMonths(nanny.minAgeOfKid)) <= 0)
            {
                throw new Exception("cannot sign contract:\nchild is too little.\nthose are the contract detail:\n" +
                    "***********************************************\n" + contract.ToString() + "***********************************************\n");
            }

            // checks that the child is in a age that nanny can take
            if (DateTime.Now.CompareTo(child.birthday.AddMonths(nanny.maxAgeOfKid)) >= 0)
            {
                throw new Exception("cannot sign contract:\nchild is too big.\nthose are the contract detail:\n" +
                    "***********************************************\n" + contract.ToString() + "***********************************************\n");
            }

            if (!(nanny.doesWorkPerHour || contract.isMonthContract))
            {
                throw new Exception("cannot sign contract:\nNanny work per month only!.\nthose are the contract detail:\n" +
                    "***********************************************\n" + contract.ToString() + "***********************************************\n");
            }

            nanny.numberOfSignedContracts++; // adds to nanny count of contracts

            // calculates nanny salary
            if (contract.isMonthContract)
            {
                contract.monthSalary = (float)calculateSalary(contract, nanny);
                contract.moneyPerHour = 0;
            }
            else
            {
                contract.monthSalary = 0;
                contract.moneyPerHour = (float)calculateSalary(contract, nanny);
            }

            initalizeContractNumber(contract); // gets a serial number for contract

            myDal.addContract(contract);  // goes to DAL method     
        }

        /// <summary>
        /// removes contract from list
        /// using DAL method
        /// </summary>
        /// <param name="contract"></param>
        public void deleteContract(Contract contract)
        {
            // decrease number of signed contract of nanny
            Nanny temp = GetNannyByID(contract.NannysId);
            if (temp != null)
            {
                temp.numberOfSignedContracts--;
            }
            myDal.deleteContract(contract);
        }

        /// <summary>
        /// updates the contract information
        /// using DAL method
        /// </summary>
        /// <param name="contract"></param>
        public void updateContract(Contract contract)
        {
            myDal.updateContract(contract);
        }

        /// <summary>
        /// gets list of nanny's
        /// using DAL method
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Nanny> getListOfNanny()
        {
            return myDal.getListOfNanny();
        }

        /// <summary>
        /// gets list of mothers
        /// using DAL method
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Mother> getListOfMother()
        {
            return myDal.getListOfMother();
        }

        /// <summary>
        /// gets list of children
        /// using DAL method
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Child> getListOfChild()
        {
            return myDal.getListOfChild();
        }

        /// <summary>
        /// gets list off all singed contracts 
        /// using DAL method
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Contract> getListOfContract()
        {
            return myDal.getListOfContract();
        }

        /// <summary>
        /// scan all ids in all lists
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IdExist(int id)
        {
            return myDal.IdExist(id);
        }

        /// <summary>
        /// gives a new contract a serial number
        /// using DAL method
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        public bool initalizeContractNumber(Contract contract)
        {
            return myDal.initalizeContractNumber(contract);
        }

        /// <summary>
        /// checks if nanny is on list
        /// using DAL method
        /// </summary>
        /// <param name="nanny"></param>
        /// <returns></returns>
        public bool isNannyInList(int nanny)
        {
            return myDal.isNannyInList(nanny);
        }

        /// <summary>
        /// checks if mother is on list
        /// using DAL method
        /// </summary>
        /// <param name="mother"></param>
        /// <returns></returns>
        public bool isMotherInList(int mother)
        {
            return myDal.isMotherInList(mother);
        }

        /// <summary>
        /// checks if child is on list
        /// using DAL method
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public bool isChildInList(int child)
        {
            return myDal.isChildInList(child);
        }

        /// <summary>
        /// checks if a contract is on list 
        /// using DAL method
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        public bool isContractInList(Contract contract)
        {
            return myDal.isContractInList(contract);
        }

        /// <summary>
        ///  returns the object of child from list by searching is id
        ///  using DAL method
        /// </summary>
        /// <param name="childId"></param>
        /// <returns></returns>
        public Child getChildByID(int childId)
        {
            var temp = from item in myDal.getListOfChild()
                       where item.id == childId
                       select item;

            return temp.ElementAt(0);
        }

        /// <summary>
        /// gets a Mother object by searching her id number
        /// using DAL method
        /// </summary>
        /// <param name="motherId"></param>
        /// <returns></returns>
        public Mother getMotherByID(int motherId)
        {
            var temp = from item in myDal.getListOfMother()
                       where item.id == motherId
                       select item;

            return temp.ElementAt(0);
        }

        /// <summary>
        ///  returns the object of nanny from list by searching is id
        ///  using DAL method
        /// </summary>
        /// <param name="nannyId"></param>
        /// <returns></returns>
        public Nanny GetNannyByID(int nannyId)
        {
            var temp = from item in myDal.getListOfNanny()
                       where item.id == nannyId
                       select item;

            return temp.ElementAt(0);
        }

        /// <summary>
        ///  returns the object of mother from list by searching her childes id
        ///  using DAL method
        /// </summary>
        /// <param name="childId"></param>
        /// <returns></returns>
        public Mother GetMotherByChildID(int childId)
        {
            int motherId = myDal.getMotherId(childId);
            IEnumerable<Mother> myMother = from item in myDal.getListOfMother()
                                           where item.id == motherId
                                           select item;
            return myMother.ElementAt(0);
        }

        /// <summary>
        /// gets sum of Nanny caring hours in MONTH.
        /// </summary>
        /// <returns></returns>
        public float totalHoursByNanny(Mother mother)
        {
            float sum = 0;
            int index = 0;
            foreach (Day temp in mother.hoursByNanny)
            {
                if (mother.daysOfNanny[index])
                    sum += temp.workTime();
                index++;
            }
            sum *= 4;
            return sum;
        }

        /// <summary>
        /// calculating salary of nanny.
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="nanny"></param>
        /// <returns></returns>
        public double calculateSalary(Contract contract, Nanny nanny)
        {
            // in order to give discount we need to find number of brothers
            int numberOfBrothers = brothersByNanny(nanny, GetMotherByChildID(contract.childId).id);

            double salary = 0;
            if (contract.isMonthContract)
            {
                salary = nanny.monthlyWage;
            }

            else
            {
                salary = nanny.hourWage * totalHoursByNanny(GetMotherByChildID(contract.childId));
            }

            double coffiecient = 1;
            for (int i = 0; i < numberOfBrothers; i++)
                coffiecient = coffiecient - 0.02;

            return salary * coffiecient;
        }

        /// <summary>
        /// returns number of brothers that are cared by the same nanny
        /// </summary>
        /// <param name="nanny"></param>
        /// <param name="momsId"></param>
        /// <returns>int</returns>
        public int brothersByNanny(Nanny nanny, int momsId)
        {
            int num = 0;
            foreach (Contract c in getListOfContract())
            {
                if (c.NannysId == nanny.id && GetMotherByChildID(c.childId).id == momsId)
                {
                    num++;
                }
            }
            return num;
        }

        /// <summary>
        /// check if it can find a specific address in Google maps, by trying to find routes from "Lev Academic Center" to it.
        /// </summary>
        /// <param name="source"></param>
        public bool findAddress(string source)
        {
            float a = 0;
            Thread MyThread = new Thread(() => { a = distanceBetweenAddresses(source, "Lev Academic Center"); });
            MyThread.Start();
            MyThread.Join();
            return a >= 0;
        }

        /// <summary>
        /// use Google Api to get distance between two address stored in strings
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public float distanceBetweenAddresses(string source, string dest)
        {
            // in case at least one address is empty - return immediately
            if (source == null || dest == null)
                return -1;

            var drivingDirectionRequest = new DirectionsRequest
            {
                TravelMode = TravelMode.Walking,
                Origin = source,
                Destination = dest,
            };

            DirectionsResponse drivingDirections = GoogleMaps.Directions.Query(drivingDirectionRequest);

            if (drivingDirections.Routes.ElementAtOrDefault(0) == null)
                return -1;

            Route route = drivingDirections.Routes.First();
            Leg leg = route.Legs.First();
          
            return Convert.ToSingle(leg.Distance.Value / 1000.0);
            
            
        }

        /// <summary>
        /// returns collection of Nanny's that stands in moms hour demands. age is irrelevant.
        /// </summary>
        /// <param name="mother"></param>
        /// <returns></returns>
        public IEnumerable<Nanny> potentialNannys(Mother mother)
        {
            List<Nanny> list = new List<Nanny>();
            //List<Thread> listOfThreds = new List<Thread>();
            float distance = 0;
            foreach (Nanny nanny in myDal.getListOfNanny())
            {
                // check schedule
                bool flag = true;
                for (int i = 0; i < 6 && flag && mother.daysOfNanny[i]; i++)
                {
                    // compare the day
                    if (mother.daysOfNanny[i] && !nanny.daysOfWork[i])
                    {
                        flag = false;
                        break;
                    }

                    // compare the start hour
                    if (mother.hoursByNanny[i].start_hour == nanny.hoursOfWork[i].start_hour)
                    {
                        if (mother.hoursByNanny[i].start_minute < nanny.hoursOfWork[i].start_minute)
                            flag = false;
                    }

                    else
                    {
                        if (mother.hoursByNanny[i].start_hour < nanny.hoursOfWork[i].start_hour)
                            flag = false;
                    }

                    //Compare the finish hour
                    if (mother.hoursByNanny[i].finish_hour == nanny.hoursOfWork[i].finish_hour)
                    {
                        if (mother.hoursByNanny[i].finish_minute > nanny.hoursOfWork[i].finish_minute)
                            flag = false;
                    }

                    else
                    {
                        if (mother.hoursByNanny[i].finish_hour > nanny.hoursOfWork[i].finish_hour)
                            flag = false;
                    }
                }

                // if schedule is suitable -> check Distance
                if (flag)
                {
                    Thread myThred = new Thread(() => { distance = distanceBetweenAddresses(mother.address, nanny.address); });
                    myThred.Start();
                    myThred.Join();

                    if (distance > mother.addressRadius)
                    {
                        flag = false;
                    }
                }

                if (flag)
                    list.Add(nanny);
            }

            if (list != null)
                return list;
            else
                return fiveMostSuitableNannys(mother);
        }

        /// <summary>
        /// in case there is no completely suitable Nanny, this methods returns the 5 most Suitable Nanny's.
        /// </summary>
        /// <param name="mother"></param>
        /// <returns></returns>
        public IEnumerable<Nanny> fiveMostSuitableNannys(Mother mother)
        {
            List<Nanny> list = new List<Nanny>();
            int counter = 0;

            foreach (Nanny nanny in myDal.getListOfNanny())
            {
                bool flag = true;
                for (int i = 0; i < 6 && flag; i++)
                {
                    // compare the day
                    if (mother.daysOfNanny[i] && !nanny.daysOfWork[i])
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag && counter < 5)
                {
                    list.Add(nanny);
                    counter++;
                }
            }

            if (list != null)
                return list;
            else
                throw new Exception("no nanny was found");
        }

        /// <summary>
        /// returns Nanny's that stands in moms demands and in some range
        /// </summary>
        /// <param name="mother"></param>
        /// <param name="distance">In Kilometers</param>
        /// <returns></returns>
        public IEnumerable<Nanny> NannysAround(Mother mother, int distance)
        {
            int distanceInMeters = distance * 1000;
            var list = from nanny in myDal.getListOfNanny()
                       where distanceBetweenAddresses(mother.address, nanny.address) <= distanceInMeters
                       select nanny;

            return list;
        }

        /// <summary>
        /// search the DS list for children with no nanny.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Child> ChildrenWithoutNanny()
        {
            List<Child> listNoNanny = new List<Child>();

            foreach (Child kid in myDal.getListOfChild())
            {
                bool flag = true;
                foreach (Contract contract in myDal.getListOfContract())
                {
                    if (contract.childId == kid.id)
                        flag = false;
                }

                if (flag)
                    listNoNanny.Add(kid);
            }

            if (listNoNanny != null)
                return listNoNanny;
            else
                throw new Exception("no child with no nanny was found");
        }

        /// <summary>
        /// returns a list of all nanny's that have a government vacation days
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Nanny> NannysWithGovVacation()
        {
            return from item in myDal.getListOfNanny()
                   where item.hasGovVacationDays == true
                   select item;
        }

        /// <summary>
        /// returns all contracts numbers that fit a specific condition
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<int> GetAllContractsNumbers(Func<Contract, bool> predicate = null)
        {
            if (predicate == null) return null;
            return from contract in myDal.getListOfContract()
                   where predicate(contract)
                   select contract.numberOfContract;
        }

        /// <summary>
        /// gets all contracts that fit a specific condition
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<Contract> GetAllContracts(Func<Contract, bool> predicate = null)
        {

            if (predicate == null) return getListOfContract().AsEnumerable();
            // return getListOfContract().Where(predicate);
            return from item in myDal.getListOfContract()
                   where predicate(item)
                   select item;

            //return null;
        }

        /// <summary>
        /// return a collection of groups based on a Nanny collection. every group hold Nanny's and had a unique age of kid: minimum or maximum.
        /// </summary>
        /// <param name="collection">collection of Nanny's</param>
        /// <param name="byMinAge">boolean variable, indicate if the returned groups are different in minimum age of kid. if false: it is by max age of kid. </param>
        /// <param name="sorted">boolean variable, indicate if the returned groups are sorted</param>
        /// <returns></returns>
        public IEnumerable<IGrouping<int, Nanny>> GroupOfNannysByAgeOfKid(IEnumerable<Nanny> collection, bool byMinAge, bool sorted)
        {
            IEnumerable<IGrouping<int, Nanny>> temp;

            // grouping
            if (byMinAge)
                temp = from nanny in collection
                       group nanny by nanny.minAgeOfKid;

            else
                temp = from nanny in collection
                       group nanny by nanny.maxAgeOfKid;

            //sorting is by family name
            if (sorted)
            {
                // sort all groups
                foreach (var group in temp)
                    group.OrderBy(nanny => nanny.familyName + nanny.firstName);

                // sort the groups
                temp.OrderBy(group => group.ElementAt(0).familyName);
            }

            return temp;
        }

        /// <summary>
        /// return a collection of groups based on a Contract collection. every group hold contracts and had different distance level.
        /// </summary>
        /// <param name="collection">collection of Contracts</param>
        /// <param name="sorted">boolean variable, indicate if the returned groups are sorted</param>
        /// <returns></returns>
        public IEnumerable<IGrouping<float, Contract>> GroupOfContractsByDistance(IEnumerable<Contract> collection, bool sorted)
        {
            IEnumerable<IGrouping<float, Contract>> temp;

            // grouping
            temp = from contract in collection
                   group contract by contract.Distance;
            return temp;
        }

        /// <summary>
        /// return list of contracts that share an id number.
        /// </summary>
        /// <param name="my_id"></param>
        /// <returns></returns>
        public IEnumerable<Contract> ListOfContractsById(int my_id)
        {
            return myDal.ListOfContractsById(my_id);
        }

        /// <summary>
        /// return list of children that have the same mother.
        /// </summary>
        /// <param name="mother"></param>
        /// <returns></returns>
        public IEnumerable<Child> getListOfChildByMother(Mother mother)
        {
            return myDal.getListOfChildByMother(mother);
        }

        /// <summary>
        /// returns a list of all the children in the group of a nanny
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<Child> getListOfChildrenOfNanny(int id)
        {
            return myDal.getListOfChildrenOfNanny(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="nanny"></param>
        /// <returns></returns>
        public IEnumerable<Child> checkAgeOfKids(IEnumerable<Child> list, Nanny nanny)
        {
            return myDal.checkAgeOfKids(list, nanny);
        }

        /// <summary>
        /// gets the list of all contracts of mother
        /// using DAL method
        /// </summary>
        /// <param name="thisMother"></param>
        /// <returns></returns>
        public IEnumerable<Contract> getListOfContractByMother(Mother thisMother)
        {
            return myDal.getListOfContractByMother(thisMother);
        }
    }
}
