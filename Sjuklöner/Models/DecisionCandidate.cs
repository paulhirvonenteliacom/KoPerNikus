using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sjuklöner.Models
{
    //When the robot has transferred a claim to Procapita, the robot adds the referencenumber of the claim to this table.
    //When the robot detects that a decision has been made about a claim in Procapita, then the robot removes the referencenumber of the claim from this table. 
    public class DecisionCandidate
    {
        public int Id { get; set; }

        public string ReferenceNumber { get; set; }
    }
}